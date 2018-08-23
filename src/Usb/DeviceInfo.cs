using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dandy.Devices.Usb
{
    /// <summary>
    /// Information about a USB device.
    /// </summary>
    /// <remarks>
    /// Retrieving this info does not involve any I/O to the device.
    /// To open a device for I/O operations, use <see cref="OpenAsync"/>.
    /// </remarks>
    public sealed partial class DeviceInfo : IDisposable
    {
        /// <summary>
        /// The name of the device. Suitable for display to the user.
        /// </summary>
        public string DisplayName => _get_DisplayName();

        /// <summary>
        /// The USB vendor ID.
        /// </summary>
        public ushort VendorId => _get_VendorId();

        /// <summary>
        /// The USB product ID.
        /// </summary>
        public ushort ProductId => _get_ProductId();

        /// <summary>
        /// Opens the device for I/O. Dispose the returned device to close the I/O connection.
        /// </summary>
        /// <returns></returns>
        public Task<Device> OpenAsync() => _OpenAsync();

        /// <summary>
        /// Disposes unmanaged resources, if any.
        /// </summary>
        public void Dispose() => _Dispose();
    }
}
