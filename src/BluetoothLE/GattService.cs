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
        /// Gets the device that provides this service.
        /// </summary>
        public Device Device => _get_Device();

        /// <summary>
        /// Gets a list of characteristics for the service
        /// </summary>
        public Task<IReadOnlyList<GattCharacteristic>> GetCharacteristicsAsync(Guid uuid) => _GetCharacteristicsAsync(uuid);

        /// <inheritdoc/>
        public void Dispose() => _Dispose();
    }
}
