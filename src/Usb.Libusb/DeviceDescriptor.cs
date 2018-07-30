using System;
using System.Runtime.InteropServices;

namespace Dandy.Devices.Usb.Libusb
{
    /// <summary>
    /// Class representing the standard USB device descriptor.
    /// </summary>
    public sealed class DeviceDescriptor
    {
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
        /// USB specification release number in binary-coded decimal.
        /// </summary>
        public ushort BcdUsb => descriptor.bcdUSB; // FIXME: need to swap bytes on big-endian systems

        /// <summary>
        /// USB-IF class code for the device.
        /// </summary>
        public ClassCode DeviceClass => (ClassCode)descriptor.bDeviceClass;

        /// <summary>
        /// USB-IF subclass code for the device, qualified by the bDeviceClass value.
        /// </summary>
        public byte DeviceSubClass => descriptor.bDeviceSubClass;

        /// <summary>
        /// USB-IF protocol code for the device, qualified by the bDeviceClass
        /// and bDeviceSubClass values.
        /// </summary>
        public byte DeviceProtocol => descriptor.bDeviceProtocol;

        /// <summary>
        /// Maximum packet size for endpoint 0.
        /// </summary>
        public byte MaxPacketSize0 => descriptor.bMaxPacketSize0;

        /// <summary>
        /// USB-IF vendor ID.
        /// </summary>
        public ushort VendorId => descriptor.idVendor; // FIXME: need to swap bytes on big-endian systems

        /// <summary>
        /// USB-IF product ID.
        /// </summary>
        public ushort ProductId => descriptor.idProduct; // FIXME: need to swap bytes on big-endian systems

        /// <summary>
        /// Device release number in binary-coded decimal.
        /// </summary>
        public ushort BcdDevice => descriptor.bcdDevice; // FIXME: need to swap bytes on big-endian systems

        /// <summary>
        /// Index of string descriptor describing manufacturer.
        /// </summary>
        public byte ManufacturerIndex => descriptor.iManufacturer;

        /// <summary>
        /// Index of string descriptor describing product.
        /// </summary>
        public byte ProductIndex => descriptor.iProduct;

        /// <summary>
        /// Index of string descriptor containing device serial number.
        /// </summary>
        public byte SerialNumberIndex => descriptor.iSerialNumber;

        /// <summary>
        /// Number of possible configurations.
        /// </summary>
        public byte NumberOfConfigurations => descriptor.bNumConfigurations;

        [DllImport("usb-1.0")]
        static extern int libusb_get_device_descriptor(IntPtr dev, out Struct desc);

        internal DeviceDescriptor(Device dev)
        {
            var dev_ = dev?.Handle ?? throw new ArgumentNullException(nameof(dev));
            var ret = libusb_get_device_descriptor(dev_, out descriptor);
            if (ret < 0) {
                throw new ErrorException(ret);
            }
        }
    }
}
