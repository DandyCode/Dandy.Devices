namespace Dandy.Devices.USB.Libusb
{
    /// <summary>
    /// Indicates the speed at which the device is operating.
    /// </summary>
    public enum Speed
    {

        /// <summary>
        /// The OS doesn't report or know the device speed.
        /// </summary>
        Unknown,

        /// <summary>
        /// The device is operating at low speed (1.5MBit/s).
        /// </summary>
        Low,

        /// <summary>
        /// The device is operating at full speed (12MBit/s).
        /// </summary>
        Full,

        /// <summary>
        /// The device is operating at high speed (480MBit/s).
        /// </summary>
        High,

        /// <summary>
        /// The device is operating at super speed (5000MBit/s).
        /// </summary>
        Super,

        /// <summary>
        /// The device is operating at super speed plus (10000MBit/s).
        /// </summary>
        SuperPlus,
    }
}
