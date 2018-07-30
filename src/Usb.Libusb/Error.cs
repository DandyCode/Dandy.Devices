using System;
using System.Runtime.InteropServices;

namespace Dandy.Devices.Usb.Libusb
{
    /// <summary>
    /// Error codes.
    /// </summary>
    public enum Error
    {
        /// <summary>
        /// Success (no error)
        /// </summary>
        Success = 0,

        /// <summary>
        /// Input/output error.
        /// </summary>
        IO = -1,

        /// <summary>
        /// Invalid parameter.
        /// </summary>
        InvalidParam = -2,

        /// <summary>
        /// Access denied (insufficient permissions)
        /// </summary>
        Access = -3,

        /// <summary>
        /// No such device (it may have been disconnected)
        /// </summary>
        NoDevice = -4,

        /// <summary>
        /// Entity not found.
        /// </summary>
        NotFound = -5,

        /// <summary>
        /// Resource busy.
        /// </summary>
        Busy = -6,

        /// <summary>
        /// Operation timed out.
        /// </summary>
        Timeout = -7,

        /// <summary>
        /// Overflow.
        /// </summary>
        Overflow = -8,

        /// <summary>
        /// Pipe error.
        /// </summary>
        Pipe = -9,

        /// <summary>
        /// System call interrupted (perhaps due to signal)
        /// </summary>
        Interupted = -10,

        /// <summary>
        /// Insufficient memory.
        /// </summary>
        NoMem = -11,

        /// <summary>
        /// Operation not supported or unimplemented on this platform.
        /// </summary>
        NotSupported = -12,

        /// <summary>
        /// Other error.
        /// </summary>
        Other = -99,
    }

    /// <summary>
    /// libusb error exception
    /// </summary>
    public sealed class ErrorException : Exception
    {
        /// <summary>
        /// Gets the error code
        /// </summary>
        public Error Error { get; }

        [DllImport("usb-1.0")]
        static extern IntPtr libusb_strerror(Error error);

        static string StrError(Error error)
        {
            var ptr = libusb_strerror(error);
            // FIXME: this is UTF-8
            return Marshal.PtrToStringAnsi(ptr);
        }

        /// <summary>
        /// Creates a new libusb error exception
        /// </summary>
        /// <param name="error">the error code</param>
        public ErrorException(Error error) : base(StrError(error))
        {
            Error = error;
        }

        /// <summary>
        /// Creates a new libusb error exception
        /// </summary>
        /// <param name="error">the error code</param>
        public ErrorException(int error) : this ((Error)error)
        {
        }
    }
}
