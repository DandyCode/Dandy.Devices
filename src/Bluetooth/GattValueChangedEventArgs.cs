using System;

namespace Dandy.Devices.Bluetooth
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
