using System;
using System.Threading.Tasks;

namespace Dandy.Devices.BluetoothLE
{
    /// <summary>
    /// Class representing a GATT characteristic
    /// </summary>
    public sealed partial class GattCharacteristic : IDisposable
    {
        /// <summary>
        /// Gets the 128-bit GATT characteristic UUID.
        /// </summary>

        public Guid Uuid => _get_Uuid();

        /// <summary>
        /// Gets the service that this characteristic belongs to.
        /// </summary>
        public GattService Service => _get_Service();

        /// <summary>
        /// Issues a request to write the value of the characteristic.
        /// </summary>
        public Task WriteValueAsync(ReadOnlyMemory<byte> data, GattWriteOption option = GattWriteOption.WriteWithResponse) => _WriteValueAsync(data, option);

        /// <summary>
        /// Issues a request to read the value of the characteristic.
        /// </summary>
        public Task<Memory<byte>> ReadValueAsync() => _ReadValueAsync();

        /// <summary>
        /// Enables notifications for this characteristic.
        /// </summary>
        public Task StartNotifyAsync() => _StartNotifyAsync();

        /// <summary>
        /// Disables notifications for this characteristic.
        /// </summary>
        public Task StopNotifyAsync() => _StopNotifyAsync();

        /// <summary>
        /// Event that fires when this characteristic's value has changed.
        /// This event will on occur unless <see cref="StartNotifyAsync"/>
        /// has been called.
        /// </summary>
        public event EventHandler<GattValueChangedEventArgs> ValueChanged;

        private void OnValueChanged(Memory<byte> value)
        {
            ValueChanged?.Invoke(this, new GattValueChangedEventArgs(value));
        }
    }
}
