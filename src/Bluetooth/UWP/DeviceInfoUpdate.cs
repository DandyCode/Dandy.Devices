using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace Dandy.Devices.Bluetooth
{
    partial class DeviceInfoUpdate
    {
        internal DeviceInfoUpdate(DeviceInformationUpdate updateInfo)
        {
            UpdateInfo = updateInfo ?? throw new ArgumentNullException(nameof(updateInfo));
        }

    internal DeviceInformationUpdate UpdateInfo { get; }

    string _get_Id() => UpdateInfo.Id;

        IReadOnlyDictionary<string, object> _get_Properties() => UpdateInfo.Properties;
    }
}
