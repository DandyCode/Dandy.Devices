using System;

namespace Dandy.Devices.Bluetooth
{
    public sealed partial class BLEAdvertisementReceivedEventArgs
    {
        internal BLEAdvertisementReceivedEventArgs(object x, object y)
        {
        }

        BLEAdvertisement _get_Advertisement() => throw new NotImplementedException();

        BluetoothAddress _get_Address() => throw new NotImplementedException();
    }
}
