namespace Dandy.Devices.BluetoothLE
{
    /// <summary>
    /// Possible errors that could cause an <see cref="AdvertisementWatcher"/> to stop.
    /// </summary>
    public enum AdvertisementWatcherError
    {
        /// <summary>
        /// No error occured.
        /// </summary>
        None,

        /// <summary>
        /// An unknown error occured.
        /// </summary>
        Unknown,

        /// <summary>
        /// Don't have permission to perform the operation.
        /// </summary>
        Unauthorized,

        /// <summary>
        /// Opeation is not supported on the current hardware.
        /// </summary>
        Unsupported,

        /// <summary>
        /// The radio was turned off.
        /// </summary>
        TurnedOff,
    }
}
