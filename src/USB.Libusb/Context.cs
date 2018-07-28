
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Dandy.Devices.USB.Libusb
{
    /// <summary>
    /// Class representing a libusb session.
    /// </summary>
    sealed class Context : IDisposable
    {
        IntPtr context;

        /// <summary>
        /// The global context used by the managed wrapper for libusb.
        /// </summary>
        public static Context Global { get; } = new Context();

        public IntPtr Handle => context == IntPtr.Zero ? throw new ObjectDisposedException(null) : context;

        [DllImport("usb-1.0", CallingConvention = CallingConvention.Cdecl)]
        static extern int libusb_init(ref IntPtr context);

        Context()
        {
            var ret = libusb_init(ref context);
            if (ret < 0) {
                throw new ErrorException(ret);
            }
        }

        ~Context()
        {
            Dispose(false);
        }

        [DllImport("usb-1.0", CallingConvention = CallingConvention.Cdecl)]
        static extern void libusb_exit(IntPtr context);

        void Dispose(bool disposing)
        {
            if (context != IntPtr.Zero) {
                libusb_exit(context);
                context = IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
