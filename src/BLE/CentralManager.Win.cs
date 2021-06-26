// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

// https://github.com/dotnet/roslyn/issues/54103
#pragma warning disable CS1591

using System.Reactive.Linq;
using Windows.Foundation;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Dandy.Devices.BLE
{
    partial class CentralManager
    {
        private readonly BluetoothLEAdvertisementWatcher watcher;
        private readonly IObservable<BluetoothLEAdvertisementReceivedEventArgs> receivedObservable;
        private readonly IObservable<BluetoothLEAdvertisementWatcherStoppedEventArgs> stoppedObservable;

        private CentralManager()
        {
            watcher = new BluetoothLEAdvertisementWatcher {
                ScanningMode = BluetoothLEScanningMode.Active,
            };

            receivedObservable = Observable.FromEvent<
                TypedEventHandler<BluetoothLEAdvertisementWatcher,
                BluetoothLEAdvertisementReceivedEventArgs
            >, BluetoothLEAdvertisementReceivedEventArgs>(
                a => new TypedEventHandler<
                    BluetoothLEAdvertisementWatcher, BluetoothLEAdvertisementReceivedEventArgs
                >((s, e) => a(e)),
                h => watcher.Received += h, h => watcher.Received -= h
            );

            stoppedObservable = Observable.FromEvent<
                TypedEventHandler<BluetoothLEAdvertisementWatcher,
                BluetoothLEAdvertisementWatcherStoppedEventArgs
            >, BluetoothLEAdvertisementWatcherStoppedEventArgs>(
                a => new TypedEventHandler<
                    BluetoothLEAdvertisementWatcher, BluetoothLEAdvertisementWatcherStoppedEventArgs
                >((s, e) => a(e)),
                h => watcher.Stopped += h, h => watcher.Stopped -= h
            );
        }

        public static partial Task<CentralManager> NewAsync()
        {
            // TODO: check for Bluetooth available and powered on.
            return Task.FromResult(new CentralManager());
        }

        private sealed record Stopper(IDisposable Subscription, CentralManager Central) : IAsyncDisposable
        {
            public async ValueTask DisposeAsync()
            {
                if (Central.watcher.Status == BluetoothLEAdvertisementWatcherStatus.Stopped) {
                    Subscription.Dispose();
                    return;
                }

                var awaiter = Central.stoppedObservable.FirstAsync().GetAwaiter();

                Central.watcher.Stop();

                await awaiter;

                Subscription.Dispose();
            }
        }

        public partial Task<IAsyncDisposable> ScanAsync(
            IObserver<AdvertisementData> observer,
            IEnumerable<Guid>? uuids,
            bool filterDuplicates)
        {

            var receivedSubscription = receivedObservable
                .ApplyFilter(uuids, filterDuplicates)
                .Subscribe(a => observer.OnNext(a));

            var stoppedSubscription = stoppedObservable.FirstAsync().Subscribe(e => {
                receivedSubscription.Dispose();
                if (e.Error == BluetoothError.Success) {
                    observer.OnCompleted();
                }
                else {
                    observer.OnError(Platform.BluetoothErrorToException(e.Error));
                }
            });

            watcher.Start();

            return Task.FromResult<IAsyncDisposable>(new Stopper(stoppedSubscription, this));
        }

        public async partial Task<Peripheral> ConnectAsync(string id, CancellationToken token)
        {
            var device = await BluetoothLEDevice.FromBluetoothAddressAsync(ulong.Parse(id));
            var session = await GattSession.FromDeviceIdAsync(device.BluetoothDeviceId);
            session.MaintainConnection = true;

            var peripheral = new Peripheral(device, session);

            // TODO: ensure/await connected

            return peripheral;
        }

        public partial ValueTask DisposeAsync()
        {
            // TODO: what do we need to do here?
            return ValueTask.CompletedTask;
        }
    }
}
