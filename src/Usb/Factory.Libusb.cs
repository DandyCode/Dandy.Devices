using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dandy.Devices.Usb
{
    partial class Factory
    {
        /// <inheritdoc/>
        static Task<IEnumerable<DeviceInfo>> _FindAllAsync()
        {
            return Task.Run(() => Dandy.Libusb.Device.List.Select(d => new DeviceInfo(d)));
        }

        /// <inheritdoc/>
        static Task<IEnumerable<DeviceInfo>> _FindAllAsync(ushort vendorId, ushort productId)
        {
            return Task.Run(() => Dandy.Libusb.Device.List
                .Where(d => d.DeviceDescriptor.VendorId == vendorId && d.DeviceDescriptor.ProductId == productId)
                .Select(d => new DeviceInfo(d)));
        }
    }
}
