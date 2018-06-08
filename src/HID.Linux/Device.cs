using System;
using System.Collections.Generic;
using System.Text;
using Dandy.Devices.HID;

namespace Dandy.Devices
{
    sealed class Device : IDevice
    {
        private readonly Linux.Udev.Device device;

        public Device(Linux.Udev.Device device)
        {
            this.device = device ?? throw new ArgumentNullException(nameof(device));
        }

        public string Id => device.SysPath;

        public string DisplayName => device["HID_NAME"];

        public ushort UsagePage => throw new NotImplementedException();

        public ushort UsageId => throw new NotImplementedException();

        public ushort VendorId => throw new NotImplementedException();

        public ushort ProductId => throw new NotImplementedException();

        public ushort Version => throw new NotImplementedException();
    }
}
