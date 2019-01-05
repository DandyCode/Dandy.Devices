namespace Dandy.Devices.BluetoothLE
{
    /// <summary>
    /// Event args for <see cref="AdvertisementWatcher.Stopped"/>.
    /// </summary>
    public class AdvertisementWatcherStoppedEventArgs
    {
        /// <summary>
        /// Gets the error value.
        /// </summary>
        public AdvertisementWatcherError Error { get; }

        internal AdvertisementWatcherStoppedEventArgs(AdvertisementWatcherError error)
        {
            Error = error;
        }
    }
}
