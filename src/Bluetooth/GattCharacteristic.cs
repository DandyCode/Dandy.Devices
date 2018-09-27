using System;
using System.Threading.Tasks;

namespace Dandy.Devices.Bluetooth
{
    /// <summary>
    /// Class representing a GATT characteristic
    /// </summary>
    public sealed partial class GattCharacteristic : IDisposable
    {
        /// <summary>
        /// Gets the 128-bit GATT characteristic UUID
        /// </summary>

        public Guid Uuid => _get_Uuid();

        /// <summary>
        /// Issues a request to write the value of the characteristic.
        /// </summary>
        public Task WriteValueAsync(ReadOnlyMemory<byte> data, GattWriteOption option = GattWriteOption.WriteWithResponse) => _WriteValueAsync(data, option);

        /// <summary>
        /// Issues a request to read the value of the characteristic.
        /// </summary>
        public Task<Memory<byte>> ReadValueAsync() => _ReadValueAsync();

        public Task StartNotifyAsync() => _StartNotifyAsync();

        public Task StopNotifyAsync() => _StopNotifyAsync();

        public event EventHandler<GattValueChangedEventArgs> ValueChanged {
            add => _add_ValueChanged(value);
            remove => _remove_ValueChanged(value);
        }
    }
}
