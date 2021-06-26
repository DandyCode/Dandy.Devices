// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Threading.Tasks;

namespace Dandy.Devices.BLE
{
    /// <summary>
    /// Object that represends a GATT descriptor on a peripheral.
    /// </summary>
    public sealed partial class GattDescriptor
    {
        /// <summary>
        /// Gets the GATT characteristic that this descriptor belongs to.
        /// </summary>
        public GattCharacteristic Characteristic { get; }

        /// <summary>
        /// Gets the UUID of this descriptor.
        /// </summary>
        public Guid Uuid => GetUuid();

        private partial Guid GetUuid();

        /// <summary>
        /// Reads the value of this descriptor.
        /// </summary>
        /// <returns>The value.</returns>
        public partial Task<ReadOnlyMemory<byte>> ReadAsync();

        /// <summary>
        /// Writes the value of this descriptor.
        /// </summary>
        /// <param name="value">The value.</param>
        public partial Task WriteAsync(ReadOnlyMemory<byte> value);
    }
}
