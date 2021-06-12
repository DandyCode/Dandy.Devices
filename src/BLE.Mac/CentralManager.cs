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

            var state = await instance.@delegate.UpdatedStateObservable.FirstAsync(
                x => x != CBCentralManagerState.Unknown && x != CBCentralManagerState.Resetting
            ).GetAwaiter();

            return state switch {
                CBCentralManagerState.Unsupported => throw new Exception("bluetooth unsupported"),
                CBCentralManagerState.Unauthorized => throw new Exception("bluetooth unauthorized"),
                CBCentralManagerState.PoweredOff => throw new Exception("bluetooth powered off"),
                CBCentralManagerState.PoweredOn => instance,
                _ => throw new Exception("unknown state"),
            };
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

            var delegateSubscription = @delegate.DiscoveredPeripheralsObservable.Subscribe(observer);

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
        private readonly BehaviorSubject<CBCentralManagerState> updatedStateSubject = new(CBCentralManagerState.Unknown);
        private TaskCompletionSource<CBPeripheral[]>? retrievedPeripheralsCompletion;
        private readonly Subject<AdvertisementData> discoveredPeripheralSubject = new();

        public IObservable<CBCentralManagerState> UpdatedStateObservable => updatedStateSubject.AsObservable();
        public IObservable<AdvertisementData> DiscoveredPeripheralsObservable => discoveredPeripheralSubject.AsObservable();

        public override void UpdatedState(CBCentralManager central)
        {
            updatedStateSubject.OnNext(central.State);
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

        public override void DiscoveredPeripheral(CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI)
        {
            discoveredPeripheralSubject.OnNext(new AdvertisementData(advertisementData, RSSI));
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
