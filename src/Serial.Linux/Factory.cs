using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Dandy.Linux.Udev;

namespace Dandy.Devices.Serial.Linux
{
    /// <summary>
    /// Factory implementation for Linux
    /// </summary>
    public sealed class Factory : Serial.Factory
    {
        static IEnumerable<Serial.DeviceInfo> Find(Action<Enumerator> filter = null)
        {
            using (var udev = new Context())
            using (var e = new Enumerator(udev)) {
                e.AddMatchSubsystem("tty");
                filter?.Invoke(e);
                e.ScanDevices();
                foreach (var d in e) {
                    yield return new DeviceInfo(d);
                }
            }
        }

        /// <inheritdoc/>
        public override Task<IEnumerable<Serial.DeviceInfo>> FindAllAsync()
        {
            return Task.Run(() => Find());
        }

        /// <inheritdoc/>
        public override Task<IEnumerable<Serial.DeviceInfo>> FindAllAsync(ushort vendorId, ushort productId)
        {
            return Task.Run(() => Find(e => {
                e.AddMatchProperty("ID_VENDOR_ID", string.Format("{0:X4}", vendorId));
                e.AddMatchProperty("ID_MODEL_ID", string.Format("{0:X4}", productId));
            }));
        }
    }
}
