// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;

namespace Dandy.Devices.BLE
{
    /// <summary>
    /// Utility class for working with Bluetooth UUIDs.
    /// </summary>
    public static class Uuid
    {
        /// <summary>
        /// The base 128-bit UUID for 16-bit and 32-bit UUIDs assigned by the
        /// Bluetooth SIG.
        /// </summary>
        public static readonly Guid AssignedUuidBase = new("00000000-0000-1000-8000-00805F9B34FB");

        /// <summary>
        /// Creates a new 128-bit UUID from a 16-bit UUID.
        /// </summary>
        /// <param name="uuid">The 16-bit UUID.</param>
        /// <param name="baseUuid">The base 128-bit UUID.</param>
        /// <returns>The new 128-bit UUID.</returns>
        public static Guid From16(ushort uuid, Guid baseUuid)
        {
            var bytes = baseUuid.ToByteArray();
            // big endian
            bytes[0] = (byte)uuid;
            bytes[1] = (byte)(uuid >> 8);
            return new(bytes);
        }

        /// <summary>
        /// Creates a new 128-bit UUID from a 16-bit UUID.
        /// </summary>
        /// <param name="uuid">The 16-bit UUID.</param>
        /// <returns>The new 128-bit UUID.</returns>
        /// <remarks>
        /// This uses <see cref="AssignedUuidBase"/> as the base UUID.
        /// </remarks>
        public static Guid From16(ushort uuid)
        {
            return From16(uuid, AssignedUuidBase);
        }

        /// <summary>
        /// Creates a new 128-bit UUID from a 32-bit UUID.
        /// </summary>
        /// <param name="uuid">The 32-bit UUID.</param>
        /// <param name="baseUuid">The base 128-bit UUID.</param>
        /// <returns>The new 128-bit UUID.</returns>
        public static Guid From32(uint uuid, Guid baseUuid)
        {
            var bytes = baseUuid.ToByteArray();
            // big endian
            bytes[0] = (byte)uuid;
            bytes[1] = (byte)(uuid >> 8);
            bytes[2] = (byte)(uuid >> 16);
            bytes[3] = (byte)(uuid >> 24);
            return new(bytes);
        }

        /// <summary>
        /// Creates a new 128-bit UUID from a 32-bit UUID.
        /// </summary>
        /// <param name="uuid">The 32-bit UUID.</param>
        /// <returns>The new 128-bit UUID.</returns>
        /// <remarks>
        /// This uses <see cref="AssignedUuidBase"/> as the base UUID.
        /// </remarks>
        public static Guid From32(uint uuid)
        {
            return From32(uuid, AssignedUuidBase);
        }
    }
}
