using System;
using System.Threading.Tasks;

namespace Dandy.Devices.BluetoothLE
{
    partial class GattCharacteristic
    {
        Guid _get_Uuid() => throw new NotImplementedException();

        Task _WriteValueAsync(ReadOnlyMemory<byte> data, GattWriteOption option = GattWriteOption.WriteWithResponse) => throw new NotImplementedException();

        Task<Memory<byte>> _ReadValueAsync() => throw new NotImplementedException();

        Task _StartNotifyAsync() => throw new NotImplementedException();

        Task _StopNotifyAsync() => throw new NotImplementedException();

        /// <inheritdoc/>
        public void Dispose() => throw new NotImplementedException();
    }
}
