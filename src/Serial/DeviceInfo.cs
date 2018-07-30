using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dandy.Devices.Serial
{
    /// <summary>
    /// Information about a serial communication device.
    /// </summary>
    /// <remarks>
    /// Retrieving this info does not involve any I/O to the device.
    /// To open a device for I/O operations, use <see cref="OpenAsync"/>.
    /// </remarks>
    public abstract class DeviceInfo : IDisposable
    {
        /// <summary>
        /// The name of the device. Suitable for display to the user.
        /// </summary>
        public abstract string DisplayName { get; }

        /// <summary>
        /// The name of the serial port. e.g. COM3 on windows or ttyS0 on *nix.
        /// </summary>
        public abstract string PortName { get; }

        /// <summary>
        /// The USB vendor ID, if this is a USB serial device.
        /// </summary>
        public abstract ushort UsbVendorId { get; }

        /// <summary>
        /// The USB product ID, if this is a USB serial device.
        /// </summary>
        public abstract ushort UsbProductId { get; }

        /// <summary>
        /// Opens the device for I/O. Dispose the returned device to close the I/O connection.
        /// </summary>
        /// <returns></returns>
        public abstract Task<Device> OpenAsync();

        /// <summary>
        /// Disposes unmanaged resources, if any.
        /// </summary>
        public abstract void Dispose();
    }
}
