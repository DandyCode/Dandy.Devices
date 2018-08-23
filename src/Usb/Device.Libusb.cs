using Dandy.Libusb;

namespace Dandy.Devices.Usb
{
    partial class Device
    {
        private readonly DeviceHandle handle;

        Configuration _get_CurrentConfiguration()
        {
            var config = handle.Configuration;
            if (config == 0) {
                return null;
            }
            return new Configuration(handle, config);
        }

        Interface _get_DefaultInterface()
        {
            return new Interface(handle.Device.ActiveConfigDescriptor.Interfaces[0]);
        }

        internal Device(DeviceHandle handle)
        {
            this.handle = handle ?? throw new System.ArgumentNullException(nameof(handle));
        }

        void _Dispose()
        {
            handle.Dispose();
        }
    }
}
