using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dandy.Devices.Bluetooth
{
    /// <summary>
    /// Represents a Bluetooth device.
    /// </summary>
    public sealed partial class DeviceInformation
    {
        /// <summary>
        /// Creates a device watcher for monitoring all Bluetooth devices.
        /// </summary>
        public static DeviceWatcher CreateWatcher() => _CreateWatcher();

        /// <summary>
        /// Enumerates all Bluetooth devices.
        /// </summary>
        public static Task<IEnumerable<DeviceInformation>> FindAllAsync() => _FindAllAsync();

        /// <summary>
        /// Platform-specific device identifier.
        /// </summary>
        public string Id => _get_Id();

        /// <summary>
        /// Gets name of device suitable for display to the user.
        /// </summary>
        public string Name => _get_Name();

        /// <summary>
        /// Gets a dictionary of platform-specific properties.
        /// </summary>
        public IReadOnlyDictionary<string, object> Properties => _get_Properties();

        /// <summary>
        /// Updates the properties of this device information.
        /// </summary>
        public void Update(DeviceInformationUpdate updateInfo) => _Update(updateInfo);
    }
}
