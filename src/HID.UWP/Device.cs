using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;

namespace Dandy.Devices.HID.UWP
{
    sealed class Device : IDevice
    {
        private readonly DeviceInformation info;
        private readonly HidDevice device;

        public Device(DeviceInformation info, HidDevice device)
        {
            this.info = info ?? throw new ArgumentNullException(nameof(info));
            this.device = device ?? throw new ArgumentNullException(nameof(device));
        }

        public string Id => info.Id;

        public string DisplayName => info.Name;

        public ushort UsagePage => device.UsagePage;

        public ushort UsageId => device.UsageId;

        public ushort VendorId => device.VendorId;

        public ushort ProductId => device.ProductId;

        public ushort Version => device.Version;
    }
}

