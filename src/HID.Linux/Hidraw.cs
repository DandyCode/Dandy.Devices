using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Dandy.Devices.HID.Report;
using Dandy.Linux.Input;
using Dandy.Linux.Ioctl;
using Mono.Unix;
using Mono.Unix.Native;

using static Dandy.Linux.Ioctl.Syscall;
using static Mono.Unix.Native.Syscall;

namespace Dandy.Devices.HID.Linux
{
    sealed class Hidraw : IDisposable
    {
        const int HID_MAX_DESCRIPTOR_SIZE = 4096;

        unsafe struct hidraw_report_descriptor
        {
            public uint size;
            public fixed byte value[HID_MAX_DESCRIPTOR_SIZE];
        }

        struct hidraw_devinfo
        {
            public uint bustype;
            public ushort vendor;
            public ushort product;
        }

        static readonly UIntPtr HIDIOCGRDESCSIZE = (UIntPtr)_IO.R('H', 0x01, typeof(int));
        static readonly UIntPtr HIDIOCGRDESC = (UIntPtr)_IO.R('H', 0x02, typeof(hidraw_report_descriptor));
        static readonly UIntPtr HIDIOCGRAWINFO = (UIntPtr)_IO.R('H', 0x03, typeof(hidraw_devinfo));
        static UIntPtr HIDIOCGRAWNAME(int len) => (UIntPtr)_IO.C(_IO.C_READ, 'H', 0x04, len);
        static UIntPtr HIDIOCGRAWPHYS(int len) => (UIntPtr)_IO.C(_IO.C_READ, 'H', 0x05, len);
        static UIntPtr HIDIOCSFEATURE(int len) => (UIntPtr)_IO.C(_IO.C_WRITE | _IO.C_READ, 'H', 0x06, len);
        static UIntPtr HIDIOCGFEATURE(int len) => (UIntPtr)_IO.C(_IO.C_WRITE | _IO.C_READ, 'H', 0x07, len);

        private int fd = -1;

        Hidraw(int fd)
        {
            this.fd = fd;
            lazyReportDescriptorSize = new Lazy<int>(getReportDescriptorSize);
            lazyName = new Lazy<string>(getRawName);
            lazyPhysicalLocation = new Lazy<string>(getPhysicalLocation);
            lazyInfo = new Lazy<(Bus, ushort, ushort)>(getRawInfo);
        }

        static int Open(string path)
        {
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }
            while (true) {
                var ret = open(path, OpenFlags.O_RDWR);
                if (ret == -1) {
                    var err = Stdlib.GetLastError();
                    if (err == Errno.EINTR) {
                        continue;
                    }
                    UnixMarshal.ThrowExceptionForError(err);
                }

                return ret;
            }
        }

        public Hidraw(string path) : this(Open(path))
        {
        }

        #region IDisposable Support
        void Dispose(bool disposing)
        {
            if (fd != -1) {
                while (true) {
                    var ret = close(fd);
                    if (ret == -1) {
                        var err = Stdlib.GetLastError();
                        if (err == Errno.EINTR) {
                            continue;
                        }
                        if (disposing) {
                            // only throw exception if we are not finalizing
                            UnixMarshal.ThrowExceptionForLastErrorIf(ret);
                        }
                        // TODO: should log error if we are in the finalizer
                    }
                    break;
                }
                fd = -1;
            }
        }

        ~Hidraw()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// Gets the report descriptor size for this instance
        /// </summary>
        int ReportDescriptorSize => lazyReportDescriptorSize.Value;

        readonly Lazy<int> lazyReportDescriptorSize;

        int getReportDescriptorSize()
        {
            var ret = ioctl(fd, HIDIOCGRDESCSIZE, out int size);
            if (ret < 0) {
                var err = (Errno)(-ret);
                UnixMarshal.ThrowExceptionForError(err);
            }
            return size;
        }

        public IEnumerable<ReportCollection> GetReports()
        {
            var bytes = getReportDescriptor();
            return ReportDescirptor.Parse(bytes);
        }

        public unsafe byte[] getReportDescriptor()
        {
            hidraw_report_descriptor *descriptor = stackalloc hidraw_report_descriptor[1];
            descriptor->size = (uint)ReportDescriptorSize;

            var ret = ioctl(fd, HIDIOCGRDESC, (IntPtr)descriptor);
            if (ret < 0) {
                var err = (Errno)(-ret);
                UnixMarshal.ThrowExceptionForError(err);
            }

            var bytes = new byte[descriptor->size];
            Marshal.Copy((IntPtr)descriptor->value, bytes, 0, ReportDescriptorSize);
            return bytes;
        }

        public string Name => lazyName.Value;

        readonly Lazy<string> lazyName;

        unsafe string getRawName()
        {
            const int size = 256;

            byte *buf = stackalloc byte[size];

            var ret = ioctl(fd, HIDIOCGRAWNAME(size), (IntPtr)buf);
            if (ret < 0) {
                var err = (Errno)(-ret);
                UnixMarshal.ThrowExceptionForError(err);
            }

            return Marshal.PtrToStringAnsi((IntPtr)buf);
        }

        public string PhysicalLocation => lazyPhysicalLocation.Value;

        readonly Lazy<string> lazyPhysicalLocation;

        unsafe string getPhysicalLocation()
        {
            const int size = 256;

            byte *buf = stackalloc byte[size];

            var ret = ioctl(fd, HIDIOCGRAWPHYS(size), (IntPtr)buf);
            if (ret < 0) {
                var err = (Errno)(-ret);
                UnixMarshal.ThrowExceptionForError(err);
            }

            return Marshal.PtrToStringAnsi((IntPtr)buf);
        }

        public Bus BusType => lazyInfo.Value.bustype;

        public ushort VendorId => lazyInfo.Value.vendorId;

        public ushort ProductId => lazyInfo.Value.productId;

        readonly Lazy<(Bus bustype, ushort vendorId, ushort productId)> lazyInfo;

        unsafe (Bus bustype, ushort vendorId, ushort productId) getRawInfo()
        {
            hidraw_devinfo *info = stackalloc hidraw_devinfo[1];

            var ret = ioctl(fd, HIDIOCGRDESC, (IntPtr)info);
            if (ret < 0) {
                var err = (Errno)(-ret);
                UnixMarshal.ThrowExceptionForError(err);
            }

            return ((Bus)info->bustype, info->vendor, info->product);
        }

        public unsafe void SetFeatureReport(byte reportNumber)
        {
            const int size = 4;

            byte *buf = stackalloc byte[size];
            buf[0] = reportNumber;
            buf[1] = 0xff;
            buf[2] = 0xff;
            buf[3] = 0xff;

            var ret = ioctl(fd, HIDIOCSFEATURE(size), (IntPtr)buf);
            if (ret < 0) {
                var err = (Errno)(-ret);
                UnixMarshal.ThrowExceptionForError(err);
            }
        }

        public unsafe byte[] GetFeatureReport(byte reportNumber)
        {
            const int size = 256;

            byte *buf = stackalloc byte[size];
            buf[0] = reportNumber;

            var ret = ioctl(fd, HIDIOCGFEATURE(size), (IntPtr)buf);
            if (ret < 0) {
                var err = (Errno)(-ret);
                UnixMarshal.ThrowExceptionForError(err);
            }

            var bytes = new byte[ret];
            Marshal.Copy((IntPtr)buf, bytes, 0, ret);

            return bytes;
        }
    }
}
