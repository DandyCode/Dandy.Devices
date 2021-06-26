// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dandy.Devices.BLE
{
    /// <summary>
    /// Object that represents a Bluetooth Low Energy peripheral.
    /// </summary>
    public sealed partial class Peripheral : IAsyncDisposable
    {
        /// <summary>
        /// Gets a unique identifier for this peripheral.
        /// </summary>
        /// <remarks>
        /// This ID is suitable for "remembering" a peripheral and connecting
        /// to it when the app launches again later. It should not be shown
        /// to the user (except for debugging purposes).
        /// </remarks>
        public string Id => GetId();

        private partial string GetId();

        /// <summary>
        /// Gets the name of this peripheral.
        /// </summary>
        /// <remarks>
        /// This should be used instead of querying the Device Information service directly.
        /// </remarks>
        public string? Name => GetName();

        private partial string? GetName();

        /// <summary>
        /// Gets a list of GATT services provided by this peripheral.
        /// </summary>
        /// <param name="uuids">
        /// An optional list of service UUIDs to match. If omitted, all services
        /// will be returned.
        /// </param>
        /// <returns>A list of matching services.</returns>
        public partial Task<IEnumerable<GattService>> GetServicesAsync(IEnumerable<Guid>? uuids);

        /// <summary>
        /// Gets a list of GATT services provided by this peripheral.
        /// </summary>
        /// <param name="uuids">
        /// An optional list of service UUIDs to match. If omitted, all services
        /// will be returned.
        /// </param>
        /// <returns>A list of matching services.</returns>
        public Task<IEnumerable<GattService>> GetServicesAsync(params Guid[] uuids)
        {
            return GetServicesAsync((IEnumerable<Guid>?)uuids);
        }

        /// <summary>
        /// Disconnects this peripheral.
        /// </summary>
        public partial ValueTask DisposeAsync();
    }
}
