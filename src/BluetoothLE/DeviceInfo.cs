using System.Collections.Generic;

namespace Dandy.Devices.BluetoothLE
{
    public sealed partial class DeviceInfo
    {
        public string Id => _get_Id();

        public string Name => _get_Name();

        public IReadOnlyDictionary<string, object> Properties => _get_Properties();
    }
}
