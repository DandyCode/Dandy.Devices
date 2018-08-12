using System;

namespace Dandy.Devices.Bluetooth
{
    /// <summary>
    /// Event args for a Bluetooth Low Energy advertisement
    /// </summary>
    public sealed partial class BLEAdvertisementReceivedEventArgs
    {
        BLEAdvertisementReceivedEventArgs() => throw new NotSupportedException();

        /// <summary>
        /// Gets the advertisement
        /// </summary>
        public Advertisement Advertisement => _get_Advertisement();

        /// <summary>
        /// Gets the Bluetooth Address of the advertiser
        /// </summary>
        public BluetoothAddress Address => _get_Address();
    }
}
