using System;
using Windows.Devices.Bluetooth.Advertisement;

namespace Dandy.Devices.Bluetooth
{
    partial class BLEAdvertisementStoppedEventArgs
    {
        private readonly BluetoothLEAdvertisementWatcherStoppedEventArgs args;

        internal BLEAdvertisementStoppedEventArgs(BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            this.args = args ?? throw new ArgumentNullException(nameof(args));
        }
    }
}
