using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

namespace Dandy.Devices.Serial.Uwp
{
    /// <summary>
    /// Wrapper around Windows.Devices.Enumeration.DeviceInfo.
    /// </summary>
    public sealed class DeviceInfo : Serial.DeviceInfo
    {
        internal const string PortNameKey = "System.DeviceInterface.Serial.PortName";
        internal const string UsbVendorIdKey = "System.DeviceInterface.Serial.UsbVendorId";
        internal const string UsbProductIdKey = "System.DeviceInterface.Serial.UsbProductId";

        private readonly DeviceInformation info;

        /// <inheritdoc/>
        public override string DisplayName => info.Name;

        /// <inheritdoc/>
        public override string PortName => (string)info.Properties[PortNameKey];

        /// <inheritdoc/>
        public override ushort UsbVendorId => (ushort)info.Properties[UsbVendorIdKey];

        /// <inheritdoc/>
        public override ushort UsbProductId => (ushort)info.Properties[UsbProductIdKey];

        internal DeviceInfo(DeviceInformation info)
        {
            this.info = info ?? throw new ArgumentNullException(nameof(info));
        }

        /// <inheritdoc/>
        public override async Task<Serial.Device> OpenAsync()
        {
            var d = await SerialDevice.FromIdAsync(info.Id);
            if (d == null) {
                return null;
            }
            return new Device(d);
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
        }
    }
}
