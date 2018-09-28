using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;

namespace Dandy.Devices.BluetoothLE
{
    partial class AdvertisementWatcher
    {
        private readonly BluetoothLEAdvertisementWatcher watcher;

        AdvertisementWatcher(object obj)
        {
            watcher = new BluetoothLEAdvertisementWatcher();
            if (obj is IEnumerable<Guid> uuids) {
                foreach (var uuid in uuids) {
                    watcher.AdvertisementFilter.Advertisement.ServiceUuids.Add(uuid);
                }
            }
            watcher.Received += Watcher_Received;
            watcher.Stopped += Watcher_Stopped;
        }

        private void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            var ad = new Advertisement(args.Advertisement);
            var addr = BluetoothAddress.FromULong(args.BluetoothAddress);
            OnReceived(ad, addr, args.RawSignalStrengthInDBm);
        }

        private void Watcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            OnStopped(args.Error.ToAdvertisementWatcherError());
        }

        void _Start() => watcher.Start();

        void _Stop() => watcher.Stop();
    }

    static class AdvertisementWatcherExtensions
    {
        public static AdvertisementWatcherError ToAdvertisementWatcherError(this BluetoothError error)
        {
            switch (error) {
            case BluetoothError.Success:
                return AdvertisementWatcherError.None;
            default:
                return AdvertisementWatcherError.Unknown;
            case BluetoothError.ConsentRequired:
                return AdvertisementWatcherError.Unauthorized;
            case BluetoothError.NotSupported:
                return AdvertisementWatcherError.Unsupported;
            case BluetoothError.RadioNotAvailable:
                return AdvertisementWatcherError.TurnedOff;
            }
        }
    }
}
