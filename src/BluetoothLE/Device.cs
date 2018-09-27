using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dandy.Devices.BluetoothLE
{
    /// <summary>
    /// Class that represents a Bluetooth device.
    /// </summary>
    public sealed partial class Device : IDisposable
    {
        /// <summary>
        /// Gets the Bluetooth address of the adapter.
        /// </summary>
        public BluetoothAddress BluetoothAddress => _get_BluetoothAddress();

        /// <summary>
        /// Gets an instance of a Bluetooth adapter from a platform-specific id.
        /// </summary>
        public static Task<Device> FromIdAsync(string id) => _FromIdAsync(id);

        /// <summary>
        /// Gets a list of GATT services for the specified UUID.
        /// </summary>
        public Task<IReadOnlyList<GattService>> GetGattServicesAsync(Guid uuid) => _GetGattServicesAsync(uuid);
    }
}
