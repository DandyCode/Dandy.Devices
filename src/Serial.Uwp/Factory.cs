using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

using static Dandy.Devices.Serial.Uwp.DeviceInfo;

namespace Dandy.Devices.Serial.Uwp
{
    /// <summary>
    /// Factory for creating UWP serial communication device instances.
    /// </summary>
    public sealed class Factory : Serial.Factory
    {
        static async Task<IEnumerable<Serial.DeviceInfo>> FindAllAsync(string aqs)
        {
            var devices = await DeviceInformation.FindAllAsync(aqs, new[] { PortNameKey, UsbVendorIdKey, UsbProductIdKey });
            return devices.Select(d => new DeviceInfo(d));
        }

        /// <inheritdoc/>
        public override Task<IEnumerable<Serial.DeviceInfo>> FindAllAsync()
        {
            var aqs = SerialDevice.GetDeviceSelector();
            return FindAllAsync(aqs);
        }

        /// <inheritdoc/>
        public override Task<IEnumerable<Serial.DeviceInfo>> FindAllAsync(ushort vendorId, ushort productId)
        {
            var aqs = SerialDevice.GetDeviceSelectorFromUsbVidPid(vendorId, productId);
            return FindAllAsync(aqs);
        }
    }
}
