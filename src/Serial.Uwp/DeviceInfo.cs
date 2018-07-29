using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

namespace Dandy.Devices.Serial.Uwp
{
    public sealed class DeviceInfo : IDeviceInfo
    {
        const string PortNameKey = "System.DeviceInterface.Serial.PortName";
        const string UsbVendorIdKey = "System.DeviceInterface.Serial.UsbVendorId";
        const string UsbProductIdKey = "System.DeviceInterface.Serial.UsbProductId";

        private readonly DeviceInformation info;

        /// <inheritdoc/>
        public string DisplayName => info.Name;

        /// <inheritdoc/>
        public string PortName => (string)info.Properties[PortNameKey];

        /// <inheritdoc/>
        public ushort UsbVendorId => (ushort)info.Properties[UsbVendorIdKey];

        /// <inheritdoc/>
        public ushort UsbProductId => (ushort)info.Properties[UsbProductIdKey];

        public static async Task<IEnumerable<IDeviceInfo>> FindAllAsync()
        {
            var aqs = SerialDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync(aqs, new[] { PortNameKey, UsbVendorIdKey, UsbProductIdKey });
            return devices.Select(d => new DeviceInfo(d));
        }

        DeviceInfo(DeviceInformation info)
        {
            this.info = info ?? throw new ArgumentNullException(nameof(info));
        }

        public async Task<IDevice> OpenAsync()
        {
            var d = await SerialDevice.FromIdAsync(info.Id);
            if (d == null) {
                return null;
            }
            return new Device(d);
        }

        public void Dispose()
        {
        }
    }
}
