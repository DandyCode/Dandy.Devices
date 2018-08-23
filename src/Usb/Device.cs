using System;
using System.IO;

namespace Dandy.Devices.Usb
{
    /// <summary>
    /// Abstract class that represents a USB device.
    /// </summary>
    public sealed partial class Device : IDisposable
    {
        /// <summary>
        /// Gets the currently active configuration of the USB device.
        /// </summary>
        public Configuration CurrentConfiguration => _get_CurrentConfiguration();

        /// <summary>
        /// Gets the default interface for the current configuration.
        /// </summary>
        public Interface DefaultInterface => _get_DefaultInterface();

        /// <summary>
        /// Close the I/O connection to the device.
        /// </summary>
        public void Dispose() => _Dispose();
    }
}
