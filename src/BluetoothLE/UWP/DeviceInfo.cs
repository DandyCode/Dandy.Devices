using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Enumeration;

namespace Dandy.Devices.BluetoothLE
{
    partial class DeviceInfo
    {
        private readonly DeviceInformation info;

        string _get_Id() => info.Id;

        string _get_Name() => info.Name;

        IReadOnlyDictionary<string, object> _get_Properties() => info.Properties;

        internal DeviceInfo(DeviceInformation info)
        {
            this.info = info ?? throw new ArgumentNullException(nameof(info));
        }
    }
}
