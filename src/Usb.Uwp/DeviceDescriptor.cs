using System;
using Windows.Devices.Usb;

namespace Dandy.Devices.Usb.Uwp
{
    public sealed class DeviceDescriptor : IDeviceDescriptor
    {
        private readonly UsbDeviceDescriptor descriptor;

        internal DeviceDescriptor(UsbDeviceDescriptor descriptor)
        {
            this.descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
        }

        public ushort BcdDeviceRevision => (ushort)descriptor.BcdDeviceRevision;

        public ushort BcdUsb => (ushort)descriptor.BcdUsb;

        public byte MaxPacketSize0 => descriptor.MaxPacketSize0;

        public byte NumberOfConfigurations => descriptor.NumberOfConfigurations;

        public ushort ProductId => (ushort)descriptor.ProductId;

        public ushort VendorId => (ushort)descriptor.VendorId;
    }
}
