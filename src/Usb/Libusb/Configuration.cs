using Dandy.Libusb;

namespace Dandy.Devices.Usb
{
    partial class Configuration
    {
        private readonly ConfigDescriptor descriptor;

        internal Configuration(DeviceHandle handle, int configuration)
        {
            descriptor = handle.Device.GetConfigDescriptor(configuration);
        }
    }
}
