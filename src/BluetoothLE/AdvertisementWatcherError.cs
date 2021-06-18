namespace Dandy.Devices.BluetoothLE
{
    /// <summary>
    /// Possible errors that could cause an <see cref="AdvertisementWatcher"/> to stop.
    /// </summary>
    public enum AdvertisementWatcherError
    {
        /// <summary>
        /// No error occurred.
        /// </summary>
        None,

        /// <summary>
        /// An unknown error occurred.
        /// </summary>
        Unknown,

        /// <summary>
        /// Don't have permission to perform the operation.
        /// </summary>
        Unauthorized,

        /// <summary>
        /// Operation is not supported on the current hardware.
        /// </summary>
        Unsupported,

        /// <summary>
        /// The radio was turned off.
        /// </summary>
        TurnedOff,
    }
}
