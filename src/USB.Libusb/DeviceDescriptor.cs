using System;
using System.Runtime.InteropServices;

namespace Dandy.Devices.USB.Libusb
{
    /// <summary>
    /// Class representing the standard USB device descriptor.
    /// </summary>
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

        /// <summary>
        /// USB-IF vendor ID.
        /// </summary>
        public ushort VendorId => descriptor.idVendor; // FIXME: need to swap bytes on big-endian systems

        /// <summary>
        /// USB-IF product ID.
        /// </summary>
        public ushort ProductId => descriptor.idProduct; // FIXME: need to swap bytes on big-endian systems

        [DllImport("usb-1.0")]
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
