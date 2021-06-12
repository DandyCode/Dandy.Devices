#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CoreBluetooth;
using CoreFoundation;
using Foundation;

namespace Dandy.Devices.BLE.Mac
{
    public sealed class CentralManager : IAsyncDisposable
    {
        readonly CentralManagerDelegate @delegate;
        readonly CBCentralManager central;
        readonly IObservable<bool> isScanningObservable;
        readonly IDisposable isScanningObserver;

        private CentralManager()
        {
            @delegate = new CentralManagerDelegate();
            central = new CBCentralManager(@delegate, new DispatchQueue("Dandy.Devices.BLE.Mac"));
            var isScanningSubject = new BehaviorSubject<bool>(central.IsScanning);
            isScanningObserver = central.AddObserver("isScanning", NSKeyValueObservingOptions.New, change => {
                var value = ((NSNumber)change.NewValue).BoolValue;
                isScanningSubject.OnNext(value);
            });
            isScanningObservable = isScanningSubject.AsObservable();
        }

        public static async Task<CentralManager> NewAsync()
        {
            var instance = new CentralManager();

            for (; ; ) {
                var state = await instance.@delegate.WaitForValidStateAsync(instance.central).ConfigureAwait(false);

                switch (state) {
                case CBCentralManagerState.Resetting:
                    // wait for reset if needed
                    continue;
                case CBCentralManagerState.Unsupported:
                    throw new Exception("bluetooth unsupported");
                case CBCentralManagerState.Unauthorized:
                    throw new Exception("bluetooth unauthorized");
                case CBCentralManagerState.PoweredOff:
                    throw new Exception("bluetooth powered off");
                case CBCentralManagerState.PoweredOn:
                    return instance;
                case CBCentralManagerState.Unknown:
                    // should never be returned by WaitForValidStateAsync
                    break;
                }
                throw new Exception("unknown state");
            }
        }

        public IEnumerable<Peripheral> GetKnownPeripherals(IEnumerable<string> ids)
        {
            var uuids = ids.Select(x => new NSUuid(x)).ToArray();
            return central.RetrievePeripheralsWithIdentifiers(uuids)
                .Select(p => new Peripheral(central, @delegate, p));
        }

        public IEnumerable<Peripheral> GetKnownPeripherals(params string[] ids)
        {
            return GetKnownPeripherals(ids ?? Enumerable.Empty<string>());
        }

        public IEnumerable<Peripheral> GetConnectedPeripherals(IEnumerable<Guid> uuids)
        {
            var cbUuids = uuids.Select(x => CBUUID.FromString(x.ToString())).ToArray();
            var peripherals = central.RetrieveConnectedPeripherals(cbUuids);
            return peripherals.Select(p => new Peripheral(central, @delegate, p));
        }

        public IEnumerable<Peripheral> GetConnectedPeripherals(params Guid[] uuids)
        {
            return GetConnectedPeripherals(uuids?.AsEnumerable() ?? Enumerable.Empty<Guid>());
        }

        record ScanStopper(
            CBCentralManager central,
            IObservable<bool> isScanningObservable,
            IDisposable subscription) : IAsyncDisposable
        {
            public async Task DisposeAsync()
            {
                central.StopScan();
                await isScanningObservable.FirstAsync(x => !x).GetAwaiter();
                subscription.Dispose();
            }
        }

        public async Task<IAsyncDisposable> ScanAsync(
            IObserver<AdvertisementData> observer,
            IEnumerable<Guid>? uuids = null,
            bool filterDuplicates = true)
        {
            if (central.IsScanning) {
                throw new InvalidOperationException("already scanning");
            }

            var cbUuids = uuids?.Select(x => CBUUID.FromString(x.ToString())).ToArray();

            // filtering duplicates is the default, so we only need the options when false
            var options = filterDuplicates ? null : new NSDictionary(
                CBCentralManager.ScanOptionAllowDuplicatesKey, filterDuplicates
            );

            var delegateSubscription = @delegate.Subscribe(observer);

            central.ScanForPeripherals(cbUuids, options);
            await isScanningObservable.FirstAsync(x => x).GetAwaiter();

            // When isScanning transitions to false, observer is complete. This
            // may occur _before_ scanning is stopped via the ScanStopper, e.g.
            // if Bluetooth is turned off while scanning.
            var isScanningSubscription = isScanningObservable.FirstAsync(x => !x).Subscribe(_ => {
                delegateSubscription.Dispose();
                observer.OnCompleted();
            });

            return new ScanStopper(central, isScanningObservable, isScanningSubscription);
        }

        public Task DisposeAsync()
        {
            isScanningObserver.Dispose();

            return Task.CompletedTask;
        }
    }

    internal sealed class CentralManagerDelegate : CBCentralManagerDelegate
    {
        private TaskCompletionSource<CBCentralManagerState>? updatedStateCompletion;
        private TaskCompletionSource<CBPeripheral[]>? retrievedPeripheralsCompletion;
        private readonly List<IObserver<AdvertisementData>> discoveredObservers = new();
        private readonly object discoveredObserversLock = new();

        /// <summary>
        /// Waits for the central manager state to be something other than Unknown.
        /// </summary>
        /// <param name="central">A central manager object that is connected to this delegate.</param>
        /// <returns>The current state.</returns>
        public Task<CBCentralManagerState> WaitForValidStateAsync(CBCentralManager central)
        {
            if (central.Delegate != this) {
                throw new ArgumentException("central is not attached to delegate", nameof(central));
            }

            if (updatedStateCompletion != null) {
                throw new InvalidOperationException("already in progress");
            }

            updatedStateCompletion = new();

            if (central.State != CBCentralManagerState.Unknown) {
                updatedStateCompletion = null;
                return Task.FromResult(central.State);
            }

            return updatedStateCompletion.Task;
        }


        public override void UpdatedState(CBCentralManager central)
        {
            var completion = updatedStateCompletion;

            if (completion == null) {
                return;
            }

            completion.SetResult(central.State);
            updatedStateCompletion = null;
        }

        public Task<CBPeripheral[]> RetrievePeripheralsAsync(CBCentralManager central)
        {
            if (central.Delegate != this) {
                throw new ArgumentException("central is not attached to delegate", nameof(central));
            }

            if (retrievedPeripheralsCompletion != null) {
                throw new InvalidOperationException("already in progress");
            }

            retrievedPeripheralsCompletion = new();
            central.RetrievePeripheralsWithIdentifiers();
            return retrievedPeripheralsCompletion.Task;

        }

        public override void RetrievedPeripherals(CBCentralManager central, CBPeripheral[] peripherals)
        {
            var completion = retrievedPeripheralsCompletion;

            if (completion == null) {
                return;
            }

            completion.SetResult(peripherals);
            retrievedPeripheralsCompletion = null;
        }

        private record Unsubscriber(CentralManagerDelegate @delegate, IObserver<AdvertisementData> observer) : IDisposable
        {
            public void Dispose()
            {
                lock (@delegate.discoveredObserversLock) {
                    @delegate.discoveredObservers.Remove(observer);
                }
            }
        }

        public IDisposable Subscribe(IObserver<AdvertisementData> observer)
        {
            lock (discoveredObserversLock) {
                discoveredObservers.Add(observer);
                return new Unsubscriber(this, observer);
            }
        }

        public override void DiscoveredPeripheral(CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI)
        {
            lock (discoveredObserversLock) {
                foreach (var observer in discoveredObservers) {
                    observer.OnNext(new AdvertisementData(advertisementData, RSSI));
                }
            }
        }
    }
}

namespace System
{
    public interface IAsyncDisposable
    {
        Task DisposeAsync();
    }
}
