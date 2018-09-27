using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dandy.Devices.BluetoothLE
{
    /// <summary>
    /// Represents a Bluetooth device.
    /// </summary>
    public sealed partial class DeviceInfo
    {
        /// <summary>
        /// Creates a device watcher for monitoring all Bluetooth devices.
        /// </summary>
        public static DeviceWatcher CreateWatcher() => _CreateWatcher();

        /// <summary>
        /// Enumerates all Bluetooth devices.
        /// </summary>
        public static Task<IEnumerable<DeviceInfo>> FindAllAsync() => _FindAllAsync();

        /// <summary>
        /// Platform-specific device identifier.
        /// </summary>
        public string Id => _get_Id();

        /// <summary>
        /// Gets name of device suitable for display to the user.
        /// </summary>
        public string Name => _get_Name();

        /// <summary>
        /// Gets the Bluetooth device address
        /// </summary>
        public BluetoothAddress Address => _get_Address();

        /// <summary>
        /// Indicates if the device is currently connected.
        /// </summary>
        public bool IsConnected => _get_IsConnected();

        /// <summary>
        /// Gets a list of service UUIDs for the device
        /// </summary>
        public IReadOnlyList<Guid> ServiceUuids => _get_ServiceUuids();

        /// <summary>
        /// Gets the service advertisement data for a BLE device
        /// </summary>
        public IReadOnlyDictionary<Guid, ReadOnlyMemory<byte>> ServiceData => _get_ServiceData();

        /// <summary>
        /// Gets the manufacturer-specific advertisement data for a BLE device
        /// </summary>
        public IReadOnlyDictionary<ushort, ReadOnlyMemory<byte>> ManufacturerData => _get_ManufacturerData();

        /// <summary>
        /// Gets the advertised transmit power for a BLE device
        /// </summary>
        public short TxPower => _get_TxPower();

        /// <summary>
        /// Gets a dictionary of platform-specific properties.
        /// </summary>
        public IReadOnlyDictionary<string, object> Properties => _get_Properties();

        /// <summary>
        /// Updates the properties of this device information.
        /// </summary>
        public void Update(DeviceInfoUpdate updateInfo) => _Update(updateInfo);
    }
}
