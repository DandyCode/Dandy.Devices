using System.Collections.Generic;

namespace Dandy.Devices.Bluetooth
{
    public sealed partial class DeviceInfoUpdate
    {
        /// <summary>
        /// Platform-specific device identifier.
        /// </summary>
        public string Id => _get_Id();

        /// <summary>
        /// Gets a dictionary of platform-specific properties.
        /// </summary>
        public IReadOnlyDictionary<string, object> Properties => _get_Properties();
    }
}
