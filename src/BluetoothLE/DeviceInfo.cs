using System.Collections.Generic;

namespace Dandy.Devices.BluetoothLE
{
    /// <summary>
    /// Class containing information about a Bluetooth Low Energy device.
    /// </summary>
    public sealed partial class DeviceInfo
    {
        /// <summary>
        /// Gets a platform-specific identifier for the device.
        /// </summary>
        public string Id => _get_Id();

        /// <summary>
        /// Gets the name of the device (suitable for display to the user).
        /// </summary>
        public string Name => _get_Name();

        /// <summary>
        /// Gets a dictionary of platform-specific properties.
        /// </summary>
        public IReadOnlyDictionary<string, object> Properties => _get_Properties();
    }
}
