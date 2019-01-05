using System;

namespace Dandy.Devices.BluetoothLE
{
    /// <summary>
    /// Event args for <see cref="GattCharacteristic.ValueChanged"/>.
    /// </summary>
    public sealed class GattValueChangedEventArgs
    {
        internal GattValueChangedEventArgs(Memory<byte> value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the new characteristic value.
        /// </summary>
        public Memory<byte> Value { get; }
    }
}
