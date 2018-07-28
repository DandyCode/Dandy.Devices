using System;
using System.Runtime.InteropServices;

namespace Dandy.Devices.USB.Libusb
{
    /// <summary>
    /// Class representing a handle on a USB device.
    /// </summary>
    /// <remarks>
    /// Instances are usually obtained with <see cref="Device.Open"/>.
    ///
    /// A device handle is used to perform I/O and other operations.
    /// </remarks>
    public sealed class DeviceHandle : IDisposable
    {
        IntPtr devHandle;

        /// <summary>
        /// Pointer to the unmanaged libusb instance
        /// </summary>
        public IntPtr Handle => devHandle == IntPtr.Zero ? throw new ObjectDisposedException(null) : devHandle;

        [DllImport("usb-1.0")]
        static extern int libusb_open(IntPtr dev, out IntPtr dev_handle);

        internal DeviceHandle(Device dev)
        {
            var dev_ = dev?.Handle ?? throw new ArgumentNullException(nameof(dev));
            var ret = libusb_open(dev_, out devHandle);
            if (ret < 0) {
                throw new ErrorException(ret);
            }
        }

        [DllImport("usb-1.0")]
        static extern IntPtr libusb_open_device_with_vid_pid(IntPtr ctx, ushort vendor_id, ushort product_id);

        /// <summary>
        /// Convenience constructor for finding a device with a particular
        /// idVendor/idProduct combination.
        /// </summary>
        /// <remarks>
        /// This constructor is intended for those scenarios where you are using
        /// libusb to knock up a quick test application - it allows you to avoid
        /// calling <see cref="Device.List"/> and worrying about traversing the list.
        ///
        /// This constructors has limitations and is hence not intended for use
        /// in real applications: if multiple devices have the same IDs it will
        /// only give you the first one, etc.
        /// </remarks>
        /// <param name="vendorId">
        /// the idVendor value to search for
        /// </param>
        /// <param name="productId">
        /// the idProduct value to search for
        /// </param>
        public DeviceHandle(ushort vendorId, ushort productId)
        {
            var ctx_ = Context.Global.Handle;
            devHandle = libusb_open_device_with_vid_pid(ctx_, vendorId, productId);
            if (devHandle == IntPtr.Zero) {
                // REVISIT: could be other error as well
                throw new ErrorException(Error.NotFound);
            }
        }

        /// <inheritdoc/>
        ~DeviceHandle()
        {
            Dispose(false);
        }

        [DllImport("usb-1.0")]
        static extern void libusb_close(IntPtr list);

        void Dispose(bool disposing)
        {
            if (devHandle != IntPtr.Zero) {
                libusb_close(devHandle);
                devHandle = IntPtr.Zero;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [DllImport("usb-1.0")]
        static extern IntPtr libusb_get_device(IntPtr dev_handle);

        /// <summary>
        /// Gets the underlying device for a device handle.
        /// </summary>
        public Device Device => new Device(libusb_get_device(Handle));

        [DllImport("usb-1.0")]
        static extern int libusb_get_configuration(IntPtr dev_handle, out int config);

        [DllImport("usb-1.0")]
        static extern int libusb_set_configuration(IntPtr dev_handle, int config);

        /// <summary>
        /// Gets and sets the active configuration for a device.
        /// </summary>
        /// <remarks>
        /// You could formulate your own control request to obtain this
        /// information, but the getter has the advantage that it may be
        /// able to retrieve the information from operating system caches
        /// (no I/O involved).
        ///
        /// If the OS does not cache this information, then the getter will
        /// block while a control transfer is submitted to retrieve the
        /// information.
        ///
        /// The getter will return a value of 0 if the device is in an
        /// unconfigured state.
        ///
        /// The operating system may or may not have already set an active
        /// configuration on the device. It is up to your application to ensure
        /// the correct configuration is selected before you attempt to claim
        /// interfaces and perform other operations.
        ///
        /// If you call the setter on a device already configured with the
        /// selected configuration, then the setter will act as a lightweight
        /// device reset: it will issue a SET_CONFIGURATION request using the
        /// current configuration, causing most USB-related device state to be
        /// reset (altsetting reset to zero, endpoint halts cleared, toggles
        /// reset).
        ///
        /// You cannot change/reset configuration if your application has
        /// claimed interfaces. It is advised to set the desired configuration
        /// before claiming interfaces.
        ///
        /// Alternatively you can call <see cref="ReleaseInterface(int)"/>
        /// first. Note: if you do things this way you must ensure that
        /// <see cref="SetAutoDetachKernelDriver(bool)"/> is 0, otherwise the
        /// kernel driver will be re-attached when you release the interface(s).
        ///
        /// You cannot change/reset configuration if other applications or
        /// drivers have claimed interfaces.
        ///
        /// A configuration value of -1 will put the device in unconfigured
        /// state. The USB specifications state that a configuration value of 0
        /// does this, however buggy devices exist which actually have a
        /// configuration 0.
        ///
        /// You should always use the setter rather than formulating your own
        /// SET_CONFIGURATION control request. This is because the underlying
        /// operating system needs to know when such changes happen.
        ///
        /// The setter is a blocking call.
        /// </remarks>
        /// <value>
        /// the bConfigurationValue of the configuration you wish to activate,
        /// or -1 if you wish to put the device in an unconfigured state
        /// </value>
        public int Configuration {
            get {
                var ret = libusb_get_configuration(Handle, out var config);
                if (ret < 0) {
                    throw new ErrorException(ret);
                }
                return config;
            }
            set {
                var ret = libusb_set_configuration(Handle, value);
                if (ret < 0) {
                    throw new ErrorException(ret);
                }
            }
        }

        [DllImport("usb-1.0")]
        static extern int libusb_claim_interface(IntPtr dev_handle, int interface_number);

        /// <summary>
        /// Claims an interface on a given device handle.
        /// </summary>
        /// <remarks>
        /// You must claim the interface you wish to use before you can perform
        /// I/O on any of its endpoints.
        ///
        /// It is legal to attempt to claim an already-claimed interface, in
        /// which case libusb just returns without doing anything.
        ///
        /// If <see cref="SetAutoDetachKernelDriver(bool)"/> is set to 1 for
        /// this device, the kernel driver will be detached if necessary, on
        /// failure the detach error is returned.
        ///
        /// Claiming of interfaces is a purely logical operation; it does not
        /// cause any requests to be sent over the bus. Interface claiming is
        /// used to instruct the underlying operating system that your
        /// application wishes to take ownership of the interface.
        ///
        /// This is a non-blocking method.
        /// </remarks>
        /// <param name="interfaceNumber">
        /// the bInterfaceNumber of the interface you wish to claim
        /// </param>
        public void ClaimInterface(int interfaceNumber)
        {
            var ret = libusb_claim_interface(Handle, interfaceNumber);
            if (ret < 0) {
                throw new ErrorException(ret);
            }
        }

        [DllImport("usb-1.0")]
        static extern int libusb_release_interface(IntPtr dev_handle, int interface_number);

        /// <summary>
        /// Releases an interface previously claimed with <see cref="ClaimInterface(int)"/>.
        /// </summary>
        /// <remarks>
        /// You should release all claimed interfaces before disposing a device handle.
        ///
        /// This is a blocking method. A SET_INTERFACE control request will be
        /// sent to the device, resetting interface state to the first alternate
        /// setting.
        ///
        /// If <see cref="SetAutoDetachKernelDriver(bool)"/> is set to 1 for
        /// this device, the kernel driver will be re-attached after releasing
        /// the interface.
        /// </remarks>
        /// <param name="interfaceNumber">
        /// the bInterfaceNumber of the previously-claimed interface
        /// </param>
        public void ReleaseInterface(int interfaceNumber)
        {
            var ret = libusb_release_interface(Handle, interfaceNumber);
            if (ret < 0) {
                throw new ErrorException(ret);
            }
        }

        [DllImport("usb-1.0")]
        static extern int libusb_set_interface_alt_setting(IntPtr dev_handle, int interface_number, int alternate_setting);

        /// <summary>
        /// Activate an alternate setting for an interface.
        /// </summary>
        /// <remarks>
        /// The interface must have been previously claimed with <see cref="ClaimInterface(int)"/>
        ///
        /// You should always use this method rather than formulating your own
        /// SET_INTERFACE control request. This is because the underlying
        /// operating system needs to know when such changes happen.
        ///
        /// This is a blocking method.
        /// </remarks>
        /// <param name="interfaceNumber">
        /// the bInterfaceNumber of the previously-claimed interface
        /// </param>
        /// <param name="alternateSetting">
        /// the bAlternateSetting of the alternate setting to activate
        /// </param>
        public void SetInterfaceAlternateSetting(int interfaceNumber, int alternateSetting)
        {
            var ret = libusb_set_interface_alt_setting(Handle, interfaceNumber, alternateSetting);
            if (ret < 0) {
                throw new ErrorException(ret);
            }
        }

        [DllImport("usb-1.0")]
        static extern int libusb_clear_halt(IntPtr dev_handle,  byte endpoint);

        /// <summary>
        /// Clears the halt/stall condition for an endpoint.
        /// </summary>
        /// <remarks>
        /// Endpoints with halt status are unable to receive or transmit data
        /// until the halt condition is stalled.
        ///
        /// You should cancel all pending transfers before attempting to clear
        /// the halt condition.
        ///
        /// This is a blocking method.
        /// </remarks>
        /// <param name="endpoint">
        /// the endpoint to clear halt status
        /// </param>
        public void ClearHalt(byte endpoint)
        {
            var ret = libusb_clear_halt(Handle, endpoint);
            if (ret < 0) {
                throw new ErrorException(ret);
            }
        }

        [DllImport("usb-1.0")]
        static extern int libusb_reset_device(IntPtr dev_handle);

        /// <summary>
        /// Performs a USB port reset to reinitialize a device.
        /// </summary>
        /// <remarks>
        /// The system will attempt to restore the previous configuration and
        /// alternate settings after the reset has completed.
        ///
        /// If the reset fails, the descriptors change, or the previous state
        /// cannot be restored, the device will appear to be disconnected and
        /// reconnected. This means that the device handle is no longer valid
        /// (you should dispose it) and rediscover the device. A exception code
        /// of <see cref="Error.NotFound"/> indicates when this is the case.
        ///
        /// This is a blocking function which usually incurs a noticeable delay.
        /// </remarks>
        public void ResetDevice()
        {
            var ret = libusb_reset_device(Handle);
            if (ret < 0) {
                throw new ErrorException(ret);
            }
        }

        [DllImport("usb-1.0")]
        static extern int libusb_kernel_driver_active(IntPtr dev_handle, int interface_number);

        /// <summary>
        /// Determines if a kernel driver is active on an interface.
        /// </summary>
        /// <remarks>
        /// If a kernel driver is active, you cannot claim the interface, and
        /// libusb will be unable to perform I/O.
        ///
        /// This functionality is not available on Windows.
        /// </remarks>
        /// <param name="interfaceNumber">
        /// the interface to check
        /// </param>
        public bool IsKernelDriverActive(int interfaceNumber)
        {
            var ret = libusb_kernel_driver_active(Handle, interfaceNumber);
            if (ret < 0) {
                throw new ErrorException(ret);
            }
            return Convert.ToBoolean(ret);
        }

        [DllImport("usb-1.0")]
        static extern int libusb_detach_kernel_driver(IntPtr dev_handle, int interface_number);

        /// <summary>
        /// Detaches a kernel driver from an interface.
        /// </summary>
        /// <remarks>
        /// If successful, you will then be able to claim the interface and
        /// perform I/O.
        ///
        /// This functionality is not available on Darwin or Windows.
        ///
        /// Note that libusb itself also talks to the device through a special
        /// kernel driver, if this driver is already attached to the device,
        /// this call will not detach it and return <see cref="Error.NotFound"/>.
        /// </remarks>
        /// <param name="interfaceNumber">
        /// the interface to detach the driver from
        /// </param>
        public void DetachKernelDriver(int interfaceNumber)
        {
            var ret = libusb_detach_kernel_driver(Handle, interfaceNumber);
            if (ret < 0) {
                throw new ErrorException(ret);
            }
        }

        [DllImport("usb-1.0")]
        static extern int libusb_attach_kernel_driver(IntPtr dev_handle, int interface_number);

        /// <summary>
        /// Re-attach an interface's kernel driver, which was previously
        /// detached using <see cref="DetachKernelDriver(int)"/>.
        /// </summary>
        /// <remarks>
        /// This call is only effective on Linux and returns <see cref="Error.NotSupported"/>
        /// on all other platforms.
        ///
        /// This functionality is not available on Darwin or Windows.
        /// </remarks>
        /// <param name="interfaceNumber">
        /// the interface to attach the driver from
        /// </param>
        public void AttachKernelDriver(int interfaceNumber)
        {
            var ret = libusb_attach_kernel_driver(Handle, interfaceNumber);
            if (ret < 0) {
                throw new ErrorException(ret);
            }
        }

        [DllImport("usb-1.0")]
        static extern int libusb_set_auto_detach_kernel_driver(IntPtr dev_handle, int enable);

        /// <summary>
        /// Enables or disables libusb's automatic kernel driver detachment.
        /// </summary>
        /// <remarks>
        /// When this is enabled libusb will automatically detach the kernel
        /// driver on an interface when claiming the interface, and attach it
        /// when releasing the interface.
        ///
        /// Automatic kernel driver detachment is disabled on newly opened
        /// device handles by default.
        ///
        /// On platforms which do not have LIBUSB_CAP_SUPPORTS_DETACH_KERNEL_DRIVER
        /// this function will return <see cref="Error.NotSupported"/>, and
        /// libusb will continue as if this function was never called.
        /// </remarks>
        /// <param name="enable">
        /// whether to enable or disable auto kernel driver detachment
        /// </param>
        public void SetAutoDetachKernelDriver(bool enable)
        {
            var ret = libusb_set_auto_detach_kernel_driver(Handle, enable ? 1 : 0);
            if (ret < 0) {
                throw new ErrorException(ret);
            }
        }

        [DllImport("usb-1.0")]
        static extern int libusb_control_transfer(IntPtr dev_handle, byte bmRequestType, byte bRequest, ushort wValue, ushort wIndex, IntPtr data, ushort wLength, uint timeout);

        /// <summary>
        /// Performs a USB control transfer.
        /// </summary>
        /// <remarks>
        /// The direction of the transfer is inferred from the <paramref name="bmRequestType"/>
        /// field of the setup packet.
        /// </remarks>
        /// <param name="bmRequestType">
        /// the request type field for the setup packet
        /// </param>
        /// <param name="bRequest">
        /// the request field for the setup packet
        /// </param>
        /// <param name="wValue">
        /// the value field for the setup packet
        /// </param>
        /// <param name="wIndex">
        /// the index field for the setup packet
        /// </param>
        /// <param name="data">
        /// a suitably-sized data buffer for either input or output (depending
        /// on direction bits within <paramref name="bmRequestType"/>)
        /// </param>
        /// <param name="wLength">
        /// the length field for the setup packet. The data buffer should be at
        /// least this size.
        /// </param>
        /// <param name="timeout">
        /// timeout (in millseconds) that this function should wait before
        /// giving up due to no response being received. For an unlimited
        /// timeout, use value 0.
        /// </param>
        public void ControlTransfer(byte bmRequestType, byte bRequest, ushort wValue, ushort wIndex, byte[] data, ushort wLength, uint timeout = 0)
        {
            if (data == null) {
                throw new ArgumentNullException(nameof(data));
            }

            if (wLength > data.Length) {
                throw new ArgumentOutOfRangeException(nameof(wLength));
            }

            var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try {
                var data_ = gcHandle.AddrOfPinnedObject();
                var ret = libusb_control_transfer(Handle, bmRequestType, bRequest, wValue, wIndex, data_, wLength, timeout);
                if (ret < 0) {
                    throw new ErrorException(ret);
                }
            }
            finally {
                gcHandle.Free();
            }
        }

        [DllImport("usb-1.0")]
        static extern int libusb_bulk_transfer(IntPtr dev_handle, byte endpoint, IntPtr data, int length, out int transferred, uint timeout);

        /// <summary>
        /// Performs a USB bulk transfer.
        /// </summary>
        /// <remarks>
        /// The direction of the transfer is inferred from the direction bits
        /// of the endpoint address.
        ///
        /// For bulk reads, the length field indicates the maximum length of
        /// data you are expecting to receive. If less data arrives than
        /// expected, this function will return that data, so be sure to check
        /// the transferred output parameter.
        ///
        /// You should also check the transferred parameter for bulk writes.
        /// Not all of the data may have been written.
        ///
        /// Also check transferred when dealing with a timeout error code.
        /// libusb may have to split your transfer into a number of chunks to
        /// satisfy underlying O/S requirements, meaning that the timeout may
        /// expire after the first few chunks have completed. libusb is careful
        /// not to lose any data that may have been transferred; do not assume
        /// that timeout conditions indicate a complete lack of I/O.
        /// </remarks>
        /// <param name="endpoint">
        /// the address of a valid endpoint to communicate with
        /// </param>
        /// <param name="data">
        /// a suitably-sized data buffer for either input or output (depending
        /// on <paramref name="endpoint"/>)
        /// </param>
        /// <param name="timeout">
        /// timeout (in millseconds) that this function should wait before
        /// giving up due to no response being received. For an unlimited
        /// timeout, use value 0.
        /// </param>
        /// <returns>
        /// the number of bytes actually transferred
        /// </returns>
        public int BulkTransfer(byte endpoint, byte[] data, uint timeout = 0)
        {
            if (data == null) {
                throw new ArgumentNullException(nameof(data));
            }

            var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try {
                var data_ = gcHandle.AddrOfPinnedObject();
                var ret = libusb_bulk_transfer(Handle, endpoint, data_, data.Length, out var transferred, timeout);
                if (ret < 0) {
                    throw new ErrorException(ret);
                }
                return transferred;
            }
            finally {
                gcHandle.Free();
            }
        }

        [DllImport("usb-1.0")]
        static extern int libusb_interrupt_transfer(IntPtr dev_handle, byte endpoint, IntPtr data, int length, out int transferred, uint timeout);

        /// <summary>
        /// Performs a USB interrupt transfer.
        /// </summary>
        /// <remarks>
        /// The direction of the transfer is inferred from the direction bits
        /// of the endpoint address.
        ///
        /// For interrupt reads, the length field indicates the maximum length
        /// of data you are expecting to receive. If less data arrives than
        /// expected, this function will return that data, so be sure to check
        /// the transferred output parameter.
        ///
        /// You should also check the transferred parameter for interrupt
        /// writes. Not all of the data may have been written.
        ///
        /// Also check transferred when dealing with a timeout error code.
        /// libusb may have to split your transfer into a number of chunks to
        /// satisfy underlying O/S requirements, meaning that the timeout may
        /// expire after the first few chunks have completed. libusb is careful
        /// not to lose any data that may have been transferred; do not assume
        /// that timeout conditions indicate a complete lack of I/O.
        ///
        /// The default endpoint bInterval value is used as the polling interval.
        /// </remarks>
        /// <param name="endpoint">
        /// the address of a valid endpoint to communicate with
        /// </param>
        /// <param name="data">
        /// a suitably-sized data buffer for either input or output (depending
        /// on <paramref name="endpoint"/>)
        /// </param>
        /// <param name="timeout">
        /// timeout (in millseconds) that this function should wait before
        /// giving up due to no response being received. For an unlimited
        /// timeout, use value 0.
        /// </param>
        /// <returns>
        /// the number of bytes actually transferred
        /// </returns>
        public int InterruptTransfer(byte endpoint, byte[] data, uint timeout = 0)
        {
            if (data == null) {
                throw new ArgumentNullException(nameof(data));
            }

            var gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try {
                var data_ = gcHandle.AddrOfPinnedObject();
                var ret = libusb_interrupt_transfer(Handle, endpoint, data_, data.Length, out var transferred, timeout);
                if (ret < 0) {
                    throw new ErrorException(ret);
                }
                return transferred;
            }
            finally {
                gcHandle.Free();
            }
        }
    }
}
