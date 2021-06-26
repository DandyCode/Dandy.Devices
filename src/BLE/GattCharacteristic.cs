// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dandy.Devices.BLE
{
    /// <summary>
    /// Object that represents a GATT characteristic on a peripheral.
    /// </summary>
    public sealed partial class GattCharacteristic
    {
        /// <summary>
        /// Gets the service that this characteristic belongs to.
        /// </summary>
        public GattService Service { get; }

        /// <summary>
        /// Gets the UUID of this characteristic.
        /// </summary>
        public Guid Uuid => GetUuid();

        private partial Guid GetUuid();

        /// <summary>
        /// Gets the properties of this characteristic.
        /// </summary>
        public GattCharacteristicProperties Properties => GetProperties();

        private partial GattCharacteristicProperties GetProperties();

        /// <summary>
        /// Gets the GATT descriptors for this characteristic.
        /// </summary>
        /// <returns>A list of descriptors.</returns>
        public partial Task<IEnumerable<GattDescriptor>> GetDescriptorsAsync();

        /// <summary>
        /// Reads the value of this characteristic.
        /// </summary>
        /// <returns>The value.</returns>
        public partial Task<ReadOnlyMemory<byte>> ReadAsync();

        /// <summary>
        /// Writes a value to this characteristic.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="withResponse">
        /// If <c>false</c>, do not request a response from the peripheral.
        /// </param>
        public partial Task WriteAsync(ReadOnlyMemory<byte> value, bool withResponse = true);
    }
}
