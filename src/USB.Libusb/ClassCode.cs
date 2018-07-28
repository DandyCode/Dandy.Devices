namespace Dandy.Devices.USB.Libusb
{
    /// <summary>
    /// Device and/or Interface Class codes.
    /// </summary>
    public enum ClassCode : byte
    {
        /// <summary>
        /// In the context of a device descriptor, this bDeviceClass value
        /// indicates that each interface specifies its own class information
        /// and all interfaces operate independently.
        /// </summary>
        PerInterface = 0,

        /// <summary>
        /// Audio class.
        /// </summary>
        Audio = 1,

        /// <summary>
        /// Communications class.
        /// </summary>
        Comm = 2,

        /// <summary>
        /// Human Interface Device class.
        /// </summary>
        HID = 3,

        /// <summary>
        /// Physical.
        /// </summary>
        Physical = 5,

        /// <summary>
        /// Printer class.
        /// </summary>
        Printer = 7,

        /// <summary>
        /// Image class.
        /// </summary>
        PTP = 6,

        /// <summary>
        /// Image class.
        /// </summary>
        Image = 6,

        /// <summary>
        /// Mass storage class.
        /// </summary>
        MassStorage = 8,

        /// <summary>
        /// Hub class.
        /// </summary>
        Hub = 9,

        /// <summary>
        /// Data class.
        /// </summary>
        Data = 10,

        /// <summary>
        /// Smart Card.
        /// </summary>
        SmartCard = 0x0b,

        /// <summary>
        /// Content Security.
        /// </summary>
        ContentSecurity = 0x0d,

        /// <summary>
        /// Video.
        /// </summary>
        Video = 0x0e,

        /// <summary>
        /// Personal Healthcare.
        /// </summary>
        PersonalHealthcare = 0x0f,

        /// <summary>
        /// Diagnostic Device.
        /// </summary>
        DiagnosticDevice = 0xdc,

        /// <summary>
        /// Wireless class.
        /// </summary>
        Wireless = 0xe0,

        /// <summary>
        /// Application class.
        /// </summary>
        Application = 0xfe,

        /// <summary>
        /// Class is vendor-specific.
        /// </summary>
        VendorSpec = 0xff
    }
}
