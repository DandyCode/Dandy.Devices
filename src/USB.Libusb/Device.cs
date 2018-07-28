using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dandy.Devices.USB.Libusb
{
    /// <summary>
    /// Class representing a USB device detected on the system.
    /// </summary>
    /// <remarks>
    /// Device instances are usually obtained from <see cref="List"/>.
    ///
    /// Certain operations can be performed on a device, but in order to do any
    /// I/O you will have to first obtain a device handle using <see cref="Open"/>.
    /// </remarks>
    public sealed class Device : IDisposable
    {
        IntPtr dev;

        /// <summary>
        /// Returns a list of USB devices currently attached to the system.
        /// </summary>
        /// <remarks>
        /// This is your entry point into finding a USB device to operate.
        /// </remarks>
        public static IEnumerable<Device> List => new DeviceList();

        /// <summary>
        /// Pointer to the unmanaged instance.
        /// </summary>
        public IntPtr Handle => dev == IntPtr.Zero ? throw new ObjectDisposedException(null) : dev;

        [DllImport("usb-1.0", CallingConvention = CallingConvention.Cdecl)]
        static extern byte libusb_get_bus_number(IntPtr dev);

        /// <summary>
        /// Gets the number of the bus that a device is connected to.
        /// </summary>
        public byte BusNumber => libusb_get_bus_number(Handle);

        [DllImport("usb-1.0", CallingConvention = CallingConvention.Cdecl)]
        static extern byte libusb_get_port_number(IntPtr dev);

        /// <summary>
        /// Gets the number of the port that a device is connected to.
        /// </summary>
        /// <remarks>
        /// Unless the OS does something funky, or you are hot-plugging USB
        /// extension cards, the port number returned by this call is usually
        /// guaranteed to be uniquely tied to a physical port, meaning that
        /// different devices plugged on the same physical port should return
        /// the same port number.
        ///
        /// But outside of this, there is no guarantee that the port number
        /// returned by this call will remain the same, or even match the order
        /// in which ports have been numbered by the HUB/HCD manufacturer.
        /// </remarks>
        /// <value>
        /// the port number (0 if not available)
        /// </value>
        public byte PortNumber => libusb_get_port_number(Handle);

        [DllImport("usb-1.0", CallingConvention = CallingConvention.Cdecl)]
        static extern int libusb_get_port_numbers(IntPtr dev, IntPtr port_numbers, int port_numbers_len);

        /// <summary>
        /// Gets the list of all port numbers from root for the specified device.
        /// </summary>
        public byte[] PortNumbers {
            get {
                const int len = 7;
                var portNumbers_ = Marshal.AllocHGlobal(len);
                try {
                    var ret = libusb_get_port_numbers(Handle, portNumbers_, len);
                    if (ret < 0) {
                        throw new ErrorException(ret);
                    }
                    var portNumbers = new byte[ret];
                    Marshal.Copy(portNumbers_, portNumbers, 0, ret);
                    return portNumbers;
                }
                finally {
                    Marshal.FreeHGlobal(portNumbers_);
                }
            }
        }

        [DllImport("usb-1.0", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr libusb_get_parent(IntPtr dev);

        /// <summary>
        /// Gets the the parent from the specified device.
        /// </summary>
        /// <value>
        /// the device parent or <c>null</c> if not available
        /// </value>
        public Device Parent {
            get {
                using (new DeviceList()) {
                    var parent_ = libusb_get_parent(Handle);
                    if (parent_ == IntPtr.Zero) {
                        return null;
                    }
                    return new Device(parent_);
                }
            }
        }

        [DllImport("usb-1.0", CallingConvention = CallingConvention.Cdecl)]
        static extern byte libusb_get_device_address(IntPtr dev);

        /// <summary>
        /// Gets the address of the device on the bus it is connected to.
        /// </summary>
        public byte Address => libusb_get_device_address(Handle);

        [DllImport("usb-1.0", CallingConvention = CallingConvention.Cdecl)]
        static extern int libusb_get_device_speed(IntPtr dev);

        /// <summary>
        /// Gets the negotiated connection speed for a device.
        /// </summary>
        public Speed Speed => (Speed)libusb_get_device_speed(Handle);

        [DllImport("usb-1.0", CallingConvention = CallingConvention.Cdecl)]
        static extern int libusb_get_max_packet_size(IntPtr dev, byte endpoint);

        /// <summary>
        /// Convenience method to retrieve the wMaxPacketSize value for a
        /// particular endpoint in the active device configuration.
        /// </summary>
        /// <remarks>
        /// This function was originally intended to be of assistance when
        /// setting up isochronous transfers, but a design mistake resulted in
        /// this function instead. It simply returns the wMaxPacketSize value
        /// without considering its contents. If you're dealing with isochronous
        /// transfers, you probably want <see cref="GetMaxIsoPacketSize"/> instead.
        /// </remarks>
        /// <param name="endpoint">
        /// address of the endpoint in question
        /// </param>
        /// <returns>
        /// the wMaxPacketSize value
        /// </returns>
        public int GetMaxPacketSize(byte endpoint)
        {
            var ret = libusb_get_max_packet_size(Handle, endpoint);
            if (ret < 0) {
                throw new ErrorException(ret);
            }
            return ret;
        }

        [DllImport("usb-1.0", CallingConvention = CallingConvention.Cdecl)]
        static extern int libusb_get_max_iso_packet_size(IntPtr dev, byte endpoint);

        /// <summary>
        /// Calculates the maximum packet size which a specific endpoint is
        /// capable is sending or receiving in the duration of 1 microframe.
        /// </summary>
        /// <remarks>
        /// Only the active configuration is examined. The calculation is based
        /// on the wMaxPacketSize field in the endpoint descriptor as described
        /// in section 9.6.6 in the USB 2.0 specifications.
        ///
        /// If acting on an isochronous or interrupt endpoint, this function
        /// will multiply the value found in bits 0:10 by the number of
        /// transactions per microframe (determined by bits 11:12). Otherwise,
        /// this function just returns the numeric value found in bits 0:10.
        ///
        /// This function is useful for setting up isochronous transfers, for
        /// example you might pass the return value from this function to
        /// <see cref="SetIsoPacketLengths"/> in order to set the length field
        /// of every isochronous packet in a transfer.
        /// </remarks>
        /// <param name="endpoint">
        /// address of the endpoint in question
        /// </param>
        /// <returns>
        /// the maximum packet size which can be sent/received on this endpoint
        /// </returns>
        public int GetMaxIsoPacketSize(byte endpoint)
        {
            var ret = libusb_get_max_iso_packet_size(Handle, endpoint);
            if (ret < 0) {
                throw new ErrorException(ret);
            }
            return ret;
        }

        [DllImport("usb-1.0", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr libusb_ref_device(IntPtr dev);

        internal Device(IntPtr dev)
        {
            this.dev = libusb_ref_device(dev);
            lazyDescriptor = new Lazy<DeviceDescriptor>(() => new DeviceDescriptor(this));
        }

        /// <inheritdoc/>
        ~Device()
        {
            Dispose(false);
        }

        [DllImport("usb-1.0", CallingConvention = CallingConvention.Cdecl)]
        static extern void libusb_unref_device(IntPtr dev);

        void Dispose(bool disposing)
        {
            if (dev != IntPtr.Zero) {
                libusb_unref_device(dev);
                dev = IntPtr.Zero;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Open a device for I/O.
        /// </summary>
        /// <remarks>
        /// This is a non-blocking function; no requests are sent over the bus.
        /// </remarks>
        public DeviceHandle Open()
        {
            return new DeviceHandle(this);
        }

        /// <summary>
        /// Gets the USB device descriptor for a given device.
        /// </summary>
        public DeviceDescriptor Descriptor => lazyDescriptor.Value;
        readonly Lazy<DeviceDescriptor> lazyDescriptor;
    }
}
