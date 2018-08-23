using System;
using System.Threading.Tasks;

namespace Dandy.Devices.Usb
{
    partial class DeviceInfo
    {
        private readonly Dandy.Libusb.Device device;

        string _get_DisplayName() => throw new NotImplementedException();

        ushort _get_VendorId() => device.DeviceDescriptor.VendorId;

        ushort _get_ProductId() => device.DeviceDescriptor.ProductId;

        internal DeviceInfo(Dandy.Libusb.Device device)
        {
            this.device = device ?? throw new System.ArgumentNullException(nameof(device));
        }

        void _Dispose()
        {
            device.Dispose();
        }

        Task<Device> _OpenAsync()
        {
            return Task.FromResult(new Device(device.Open()));
        }
    }
}
