using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dandy.Devices.BluetoothLE
{
    partial class GattService
    {
        Guid _get_Uuid() => throw new NotImplementedException();

        Task<IReadOnlyList<GattCharacteristic>> _GetCharacteristicsAsync(Guid uuid) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void Dispose() => throw new NotImplementedException();
    }
}
