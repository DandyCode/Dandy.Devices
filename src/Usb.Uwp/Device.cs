using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Usb;

namespace Dandy.Devices.Usb.Uwp
{
    public sealed class Device : IDevice
    {
        private readonly DeviceInformation info;
        private readonly UsbDevice device;

        public IDeviceDescriptor DeviceDescriptor => throw new NotImplementedException();
        private readonly Lazy<DeviceDescriptor> lazyDeviceDescriptor;

        internal Device(DeviceInformation info, UsbDevice device)
        {
            this.info = info ?? throw new ArgumentNullException(nameof(info));
            this.device = device ?? throw new ArgumentNullException(nameof(device));
            lazyDeviceDescriptor = new Lazy<DeviceDescriptor>(() => new DeviceDescriptor(device.DeviceDescriptor));
        }

        #region IDisposable Support

        void Dispose(bool disposing)
        {
            if (disposing) {
                device?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
