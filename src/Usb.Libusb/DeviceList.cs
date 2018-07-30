using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dandy.Devices.Usb.Libusb
{
    sealed class DeviceList : IDisposable, IEnumerable<Device>
    {
        IntPtr list;
        IntPtr count;

        public IntPtr Handle => list == IntPtr.Zero ? throw new ObjectDisposedException(null) : list;

        [DllImport("usb-1.0")]
        static extern IntPtr libusb_get_device_list(IntPtr context, out IntPtr list);

        public DeviceList()
        {
            var ctx_ = Context.Global.Handle;
            count = libusb_get_device_list(ctx_, out list);
            if ((int)count < 0) {
                throw new ErrorException((int)count);
            }
        }

        ~DeviceList()
        {
            Dispose(false);
        }

        [DllImport("usb-1.0")]
        static extern void libusb_free_device_list(IntPtr list, int unref_devices);

        void Dispose(bool disposing)
        {
            if (list != IntPtr.Zero) {
                libusb_free_device_list(list, 1);
                list = IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        IEnumerator<Device> GetEnumerator()
        {
            for (var i = 0; i < (int)count; i++) {
                var ptr = Marshal.ReadIntPtr(Handle, i * IntPtr.Size);
                yield return new Device(ptr);
            }
        }

        IEnumerator<Device> IEnumerable<Device>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
