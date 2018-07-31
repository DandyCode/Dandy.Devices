using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Dandy.Devices.Serial.Linux
{
    public sealed class DeviceInfo : Serial.DeviceInfo
    {
        private readonly Dandy.Linux.Udev.Device device;

        public override string DisplayName { get; }

        public override string PortName => device.SysName;

        public override ushort UsbVendorId => ushort.Parse(device["ID_VENDOR_ID"], NumberStyles.HexNumber);

        public override ushort UsbProductId => ushort.Parse(device["ID_MODEL_ID"], NumberStyles.HexNumber);

        internal DeviceInfo(Dandy.Linux.Udev.Device device)
        {
            this.device = device;
            if (device["ID_VENDOR"] == device["ID_VENDOR_ID"]) {
                DisplayName = device["ID_VENDOR_FROM_DATABASE"] + " " + device["ID_MODEL_FROM_DATABASE"];
            }
            else {
                DisplayName = device["ID_VENDOR"] + " " + device["ID_MODEL"];
            }
        }

        public override void Dispose()
        {
            device.Dispose();
        }

        public override Task<Serial.Device> OpenAsync()
        {
            return Task.Run<Serial.Device>(() => new Device(device.DevNode));
        }
    }
}
