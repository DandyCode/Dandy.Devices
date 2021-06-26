// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

// https://github.com/dotnet/roslyn/issues/54103
#pragma warning disable CS1591

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

namespace Dandy.Devices.BLE
{
    partial class CentralManager
    {
        private readonly CentralManagerDelegate @delegate;
        private readonly CBCentralManager central;
        private readonly IObservable<bool> isScanningObservable;
        private readonly IDisposable isScanningObserver;

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

        public static async partial Task<CentralManager> NewAsync()
        {
            var instance = new CentralManager();

            var state = await instance.@delegate.UpdatedStateObservable.FirstAsync(
                x => x != CBManagerState.Unknown && x != CBManagerState.Resetting
            );

            return state switch {
                CBManagerState.Unsupported => throw new Exception("bluetooth unsupported"),
                CBManagerState.Unauthorized => throw new Exception("bluetooth unauthorized"),
                CBManagerState.PoweredOff => throw new Exception("bluetooth powered off"),
                CBManagerState.PoweredOn => instance,
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
        public async partial Task<Peripheral> ConnectAsync(string id, CancellationToken token)
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
            }
            catch (TaskCanceledException) {
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

        internal IEnumerable<Peripheral> GetConnectedPeripherals(IEnumerable<Guid> uuids)
        {
            var cbUuids = uuids.Select(x => CBUUID.FromString(x.ToString())).ToArray();
            var peripherals = central.RetrieveConnectedPeripherals(cbUuids);
            return peripherals.Select(p => new Peripheral(central, @delegate, p));
        }

        internal IEnumerable<Peripheral> GetConnectedPeripherals(params Guid[] uuids)
        {
            return GetConnectedPeripherals(uuids?.AsEnumerable() ?? Enumerable.Empty<Guid>());
        }

        private sealed record ScanStopper(
            CBCentralManager Central,
            IObservable<bool> IsScanningObservable,
            IDisposable Subscription) : IAsyncDisposable
        {
            public async ValueTask DisposeAsync()
            {
                Central.StopScan();
                await IsScanningObservable.FirstAsync(x => !x);
                Subscription.Dispose();
            }
        }

        public async partial Task<IAsyncDisposable> ScanAsync(
            IObserver<AdvertisementData> observer,
            IEnumerable<Guid>? uuids,
            bool filterDuplicates)
        {
            if (central.IsScanning) {
                throw new InvalidOperationException("already scanning");
            }

            var cbUuids = uuids?.Select(x => CBUUID.FromString(x.ToString())).ToArray();

            // filtering duplicates is the default, so we only need the options when false
            var options = filterDuplicates ? null : new NSDictionary(
                CBCentralManager.ScanOptionAllowDuplicatesKey, NSNumber.FromBoolean(true)
            );

            var delegateSubscription = @delegate.DiscoveredPeripheralsObservable
                .Select(x => new AdvertisementData(x.peripheral, x.advertisementData, x.rssi)).Subscribe(observer);

            central.ScanForPeripherals(cbUuids, options);
            await isScanningObservable.FirstAsync(x => x);

            // When isScanning transitions to false, observer is complete. This
            // may occur _before_ scanning is stopped via the ScanStopper, e.g.
            // if Bluetooth is turned off while scanning.
            var isScanningSubscription = isScanningObservable.FirstAsync(x => !x).Subscribe(_ => {
                delegateSubscription.Dispose();
                observer.OnCompleted();
            });

            return new ScanStopper(central, isScanningObservable, isScanningSubscription);
        }

        public partial ValueTask DisposeAsync()
        {
            isScanningObserver.Dispose();

            return ValueTask.CompletedTask;
        }
    }

    internal sealed class CentralManagerDelegate : CBCentralManagerDelegate
    {
        private readonly BehaviorSubject<CBManagerState> updatedStateSubject =
            new(CBManagerState.Unknown);
        private readonly Subject<(CBPeripheral, NSDictionary, NSNumber)> discoveredPeripheralSubject = new();
        private readonly Subject<CBPeripheral> connectedPeripheralSubject = new();
        private readonly Subject<(CBPeripheral, NSError?)> failedToConnectPeripheralSubject = new();
        private readonly Subject<(CBPeripheral, NSError?)> disconnectedPeripheralSubject = new();

        public IObservable<CBManagerState> UpdatedStateObservable =>
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
            updatedStateSubject.OnNext((CBManagerState)central.State);
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
