// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dandy.Devices.BLE
{
    /// <summary>
    /// Object that represents a GATT service of a peripheral.
    /// </summary>
    public sealed partial class GattService
    {
        /// <summary>
        /// Gets peripheral that this service belongs to.
        /// </summary>
        public Peripheral Peripheral { get; }

        /// <summary>
        /// Gets the UUID of this service.
        /// </summary>
        public Guid Uuid => GetUuid();

        private partial Guid GetUuid();

        /// <summary>
        /// Gets the characteristics for this service.
        /// </summary>
        /// <param name="uuids">
        /// Optional list of UUIDs to match. If omitted all characteristics will be returned.
        /// </param>
        /// <returns>A list of matching characteristics.</returns>
        public partial Task<IEnumerable<GattCharacteristic>> GetCharacteristicsAsync(IEnumerable<Guid>? uuids);

        /// <summary>
        /// Gets the characteristics for this service.
        /// </summary>
        /// <param name="uuids">
        /// Optional list of UUIDs to match. If omitted all characteristics will be returned.
        /// </param>
        /// <returns>A list of matching characteristics.</returns>
        public Task<IEnumerable<GattCharacteristic>> GetCharacteristicsAsync(params Guid[] uuids)
        {
            return GetCharacteristicsAsync((IEnumerable<Guid>)uuids);
        }
    }
}
