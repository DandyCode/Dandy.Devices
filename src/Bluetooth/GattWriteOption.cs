namespace Dandy.Devices.Bluetooth
{
    /// <summary>
    /// Indicates what type of write operation is to be performed.
    /// </summary>
    public enum GattWriteOption
    {
        /// <summary>
        /// The Write Without Response procedure shall be used.
        /// </summary>
        WriteWithResponse,

        /// <summary>
        /// The default GATT write procedure shall be used.
        /// </summary>
        WriteWithoutResponse,
    }
}
