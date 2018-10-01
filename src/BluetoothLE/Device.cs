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
        /// Gets the name of the device.
        /// </summary>
        public string Name => _get_Name();

        /// <summary>
        /// Gets the Bluetooth address of the device.
        /// </summary>
        public BluetoothAddress BluetoothAddress => _get_BluetoothAddress();

        /// <summary>
        /// Gets an instance of a Bluetooth LE device from a platform-specific id.
        /// </summary>
        public static Task<Device> FromIdAsync(string id) => _FromIdAsync(id);

        /// <summary>
        /// Gets an instance of a Bluetooth LE device from a Bluetooth address.
        /// </summary>
        /// <param name="address">The Bluetooth address</param>
        public static Task<Device> FromAddressAsync(BluetoothAddress address) => _FromAddressAsync(address);

        /// <summary>
        /// Gets a list of GATT services for the specified UUID.
        /// </summary>
        public Task<IReadOnlyList<GattService>> GetGattServicesAsync(Guid uuid) => _GetGattServicesAsync(uuid);
    }
}
