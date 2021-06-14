#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
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
            central = new CBCentralManager(@delegate, new DispatchQueue("Dandy.Devices.BLE.Mac.CentralManager"));
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
            );

            return state switch {
                CBCentralManagerState.Unsupported => throw new Exception("bluetooth unsupported"),
                CBCentralManagerState.Unauthorized => throw new Exception("bluetooth unauthorized"),
                CBCentralManagerState.PoweredOff => throw new Exception("bluetooth powered off"),
                CBCentralManagerState.PoweredOn => instance,
                _ => throw new Exception("unknown state"),
            };
        }

        /// <summary>
        /// Tries to connect to a BLE device.
        /// </summary>
        /// <param name="id">The OS-specific ID of the device (from <see cref="AdvertisementData"/>>.</param>
        /// <param name="token">An optional cancellation token.</param>
        /// <returns>The connected peripheral.</returns>
        /// <remarks>
        /// It is highly recommended to supply a cancellation token since this
        /// method will never time out.
        /// </remarks>
        public async Task<Peripheral> ConnectAsync(string id, CancellationToken token = default)
        {
            var cbPeripheral = central.RetrievePeripheralsWithIdentifiers(new NSUuid(id)).Single();
            var peripheral = new Peripheral(central, @delegate, cbPeripheral);

            var source = new TaskCompletionSource<Peripheral>();
            using var registration = token.Register(() => source.TrySetCanceled(token));

            using var success = @delegate.ConnectedPeripheralObservable
                .FirstAsync(x => x.Identifier == cbPeripheral.Identifier)
                .Subscribe(x => source.TrySetResult(peripheral));

            using var failure = @delegate.FailedToConnectPeripheralObservable
                .FirstAsync(x => x.peripheral.Identifier == cbPeripheral.Identifier)
                .Subscribe(x => source.TrySetException(new NSErrorException(x.error)));

            central.ConnectPeripheral(cbPeripheral);

            try {
                return await source.Task.ConfigureAwait(false);
            } catch (TaskCanceledException) {
                // The cancellation triggers a disconnect event even if the peripheral
                // was not connected.
                var awaiter = @delegate.DisconnectedPeripheralObservable
                    .FirstAsync(x => x.peripheral.Identifier == cbPeripheral.Identifier)
                    .GetAwaiter();

                central.CancelPeripheralConnection(cbPeripheral);
                await awaiter;

                throw;
            }
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

            var delegateSubscription = @delegate.DiscoveredPeripheralsObservable
                .Select(x => new AdvertisementData(x.peripheral, x.advertisementData, x.rssi)).Subscribe(observer);

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
        private readonly BehaviorSubject<CBCentralManagerState> updatedStateSubject =
            new(CBCentralManagerState.Unknown);
        private readonly Subject<(CBPeripheral, NSDictionary, NSNumber)> discoveredPeripheralSubject = new();
        private readonly Subject<CBPeripheral> connectedPeripheralSubject = new();
        private readonly Subject<(CBPeripheral, NSError?)> failedToConnectPeripheralSubject = new();
        private readonly Subject<(CBPeripheral, NSError?)> disconnectedPeripheralSubject = new();

        public IObservable<CBCentralManagerState> UpdatedStateObservable =>
            updatedStateSubject.AsObservable();
        public IObservable<(CBPeripheral peripheral, NSDictionary advertisementData, NSNumber rssi)>
            DiscoveredPeripheralsObservable => discoveredPeripheralSubject.AsObservable();
        public IObservable<CBPeripheral> ConnectedPeripheralObservable => connectedPeripheralSubject.AsObservable();
        public IObservable<(CBPeripheral peripheral, NSError? error)> FailedToConnectPeripheralObservable =>
            failedToConnectPeripheralSubject.AsObservable();
        public IObservable<(CBPeripheral peripheral, NSError? error)> DisconnectedPeripheralObservable =>
            disconnectedPeripheralSubject.AsObservable();

        public override void UpdatedState(CBCentralManager central)
        {
            updatedStateSubject.OnNext(central.State);
        }

        public override void DiscoveredPeripheral(CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI)
        {
            discoveredPeripheralSubject.OnNext((peripheral, advertisementData, RSSI));
        }

        public override void ConnectedPeripheral(CBCentralManager central, CBPeripheral peripheral)
        {
            connectedPeripheralSubject.OnNext(peripheral);
        }

        public override void FailedToConnectPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError? error)
        {
            failedToConnectPeripheralSubject.OnNext((peripheral, error));
        }

        public override void DisconnectedPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError? error)
        {
            disconnectedPeripheralSubject.OnNext((peripheral, error));
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
