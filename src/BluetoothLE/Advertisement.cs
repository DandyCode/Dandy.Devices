using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dandy.Devices.BluetoothLE
{
    /// <summary>
    /// Represents a Bluetooth device.
    /// </summary>
    public sealed partial class Advertisement
    {
        /// <summary>
        /// Gets name of device suitable for display to the user.
        /// </summary>
        public string LocalName => _get_LocalName();
        
        /// <summary>
        /// Gets a list of service UUIDs for the device
        /// </summary>
        public IReadOnlyList<Guid> ServiceUuids => _get_ServiceUuids();

        /// <summary>
        /// Gets the manufacturer-specific advertisement data for a BLE device
        /// </summary>
        public IReadOnlyDictionary<ushort, ReadOnlyMemory<byte>> ManufacturerData => _get_ManufacturerData();
    }
}
