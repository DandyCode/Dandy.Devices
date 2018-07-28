using System;
using System.Runtime.InteropServices;

namespace Dandy.Devices.USB.Libusb
{
    public sealed class DeviceDescriptor
    {
        Device device;
        Struct descriptor;

        struct Struct
        {
            #pragma warning disable CS0649
            public readonly byte bLength;
            public readonly byte bDescriptorType;
            public readonly ushort bcdUSB;
            public readonly byte bDeviceClass;
            public readonly byte bDeviceSubClass;
            public readonly byte bDeviceProtocol;
            public readonly byte bMaxPacketSize0;
            public readonly ushort idVendor;
            public readonly ushort idProduct;
            public readonly ushort bcdDevice;
            public readonly byte iManufacturer;
            public readonly byte iProduct;
            public readonly byte iSerialNumber;
            public readonly byte bNumConfigurations;
            #pragma warning restore CS0649
        }

        public ushort VendorId => descriptor.idVendor;

        public ushort ProductId => descriptor.idProduct;

        [DllImport("usb-1.0", CallingConvention = CallingConvention.Cdecl)]
        static extern int libusb_get_device_descriptor(IntPtr dev, out Struct desc);

        internal DeviceDescriptor(Device dev)
        {
            device = dev;
            var dev_ = dev?.Handle ?? throw new ArgumentNullException(nameof(dev));
            var ret = libusb_get_device_descriptor(dev_, out descriptor);
            if (ret < 0) {
                throw new ErrorException(ret);
            }
        }
    }
}
