using System;

namespace Dandy.Devices.Bluetooth
{
    /// </summary>
    /// Class representing a Bluetooth Low Energy device.
    /// </summary>
    public sealed partial class BLEDevice : IDisposable
    {
        /// <summary>
        /// Gets the Bluetooth address of the device.
        /// </summary>
        public BluetoothAddress BluetoothAddress => _get_BluetoothAddress();

        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        public string Name => _get_Name();

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        partial void Dispose(bool disposing);
    }
}
