using Windows.Devices.Bluetooth.Advertisement;

namespace Dandy.Devices.Bluetooth
{
    partial class BLEAdvertisementReceivedEventArgs
    {
        private readonly BluetoothLEAdvertisementReceivedEventArgs args;

        internal BLEAdvertisementReceivedEventArgs(BluetoothLEAdvertisementReceivedEventArgs args)
        {
            this.args = args ?? throw new System.ArgumentNullException(nameof(args));
        }

        BLEAdvertisement _get_Advertisement() => new BLEAdvertisement(args.Advertisement);

        BluetoothAddress _get_Address() => BluetoothAddress.FromULong(args.BluetoothAddress);
    }
}
