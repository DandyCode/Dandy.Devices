using System;

namespace Dandy.Devices.BluetoothLE
{
    public sealed class GattValueChangedEventArgs
    {
        internal GattValueChangedEventArgs(Memory<byte> value)
        {
            Value = value;
        }

        public Memory<byte> Value { get; }
    }
}
