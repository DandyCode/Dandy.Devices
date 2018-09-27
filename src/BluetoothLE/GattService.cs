using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dandy.Devices.BluetoothLE
{
    /// <summary>
    /// Class representing a GATT service
    /// </summary>
    public sealed partial class GattService : IDisposable
    {
        /// <summary>
        /// Gets the 128-bit GATT service UUID
        /// </summary>
        public Guid Uuid => _get_Uuid();

        /// <summary>
        /// Gets a list of characteristics for the service
        /// </summary>
        public Task<IReadOnlyList<GattCharacteristic>> GetCharacteristicsAsync() => _GetCharacteristicsAsync();
    }
}
