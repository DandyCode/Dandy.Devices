using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Dandy.Devices.BluetoothLE
{
    /// <summary>
    /// Class that represents a Bluetooth device.
    /// </summary>
    public sealed partial class Device : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        public string Name => _get_Name();

        /// <summary>
        /// Indicates if the remote device is currently connected.
        /// </summary>
        public bool IsConnected => _get_IsConnected();

        /// <summary>
        /// Gets the Bluetooth address of the device.
        /// </summary>
        public BluetoothAddress BluetoothAddress => _get_BluetoothAddress();

        /// <summary>
        /// Provides a notification when properties change.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

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

        /// <inheritdoc/>
        public void Dispose() => _Dispose();
    }
}
