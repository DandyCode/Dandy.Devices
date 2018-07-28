using System;
using System.Runtime.InteropServices;

namespace Dandy.Devices.USB.Libusb
{
    public enum Error
    {
        Success = 0,
        IO = -1,
        InvalidParam = -2,
        Access = -3,
        NoDevice = -4,
        NotFound = -5,
        Busy = -6,
        Timeout = -7,
        Overflow = -8,
        Pipe = -9,
        Interupted = -10,
        NoMem = -11,
        NotSupported = -12,
        Other = -99,
    }

    public sealed class ErrorException : Exception
    {
        public Error Error { get; }

        [DllImport("usb-1.0", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr libusb_strerror(Error error);

        static string StrError(Error error)
        {
            var ptr = libusb_strerror(error);
            // FIXME: this is UTF-8
            return Marshal.PtrToStringAnsi(ptr);
        }

        public ErrorException(Error error) : base(StrError(error))
        {
            Error = error;
        }

        public ErrorException(int error) : this ((Error)error)
        {
        }
    }
}
