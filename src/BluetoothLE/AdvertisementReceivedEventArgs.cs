namespace Dandy.Devices.BluetoothLE
{
    /// <summary>
    /// Provides data for <see cref="AdvertisementWatcher.Received"/> events.
    /// </summary>
    public sealed class AdvertisementReceivedEventArgs
    {
        /// <summary>
        /// The advertisement data.
        /// </summary>
        public Advertisement Advertisement { get; }

        /// <summary>
        /// The Bluetooth address of the advertising device.
        /// </summary>
        public BluetoothAddress Address { get; }

        /// <summary>
        /// The Received Signal Strength Indicator (RSSI) in dBm for the received advertisement.
        /// </summary>
        public short RSSI { get; }

        internal AdvertisementReceivedEventArgs(Advertisement advertisement, BluetoothAddress address, short rssi)
        {
            Advertisement = advertisement ?? throw new System.ArgumentNullException(nameof(advertisement));
            Address = address;
            RSSI = rssi;
        }
    }
}
