// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;

namespace Dandy.Devices.BLE
{
    [Flags]
    public enum GattCharacteristicProperties : ushort
    {
        /// <summary>
        /// The characteristic doesn’t have any properties that apply.
        /// </summary>
        None = 0,

        /// <summary>
        /// The characteristic supports broadcasting
        /// </summary>
        Broadcast = 1 << 0,

        /// <summary>
        /// The characteristic is readable
        /// </summary>
        Read = 1 << 1,

        /// <summary>
        /// The characteristic supports Write Without Response
        /// </summary>
        WriteWithoutResponse = 1 << 2,

        /// <summary>
        /// The characteristic is writable
        /// </summary>
        Write = 1 << 3,

        /// <summary>
        /// The characteristic is notifiable
        /// </summary>
        Notify = 1 << 4,

        /// <summary>
        /// The characteristic is indicatable
        /// </summary>
        Indicate = 1 << 5,

        /// <summary>
        /// The characteristic supports signed writes
        /// </summary>
        AuthenticatedSignedWrites = 1 << 6,

        /// <summary>
        /// The ExtendedProperties Descriptor is present
        /// </summary>
        ExtendedProperties = 1 << 7,

        /// <summary>
        /// The characteristic supports reliable writes
        /// </summary>
        ReliableWrites = 1 << 8,

        /// <summary>
        /// The characteristic has writable auxiliaries
        /// </summary>
        WritableAuxiliaries = 1 << 9,
    }
}
