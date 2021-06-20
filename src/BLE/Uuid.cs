// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;

namespace Dandy.Devices.BLE
{
    public static class Uuid
    {
        public static readonly Guid AssignedUuidBase = new("00000000-0000-1000-8000-00805F9B34FB");

        public static Guid From16(ushort uuid, Guid baseUuid)
        {
            var bytes = baseUuid.ToByteArray();
            // big endian
            bytes[0] = (byte)uuid;
            bytes[1] = (byte)(uuid >> 8);
            return new(bytes);
        }

        public static Guid From16(ushort uuid)
        {
            return From16(uuid, AssignedUuidBase);
        }

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

        public static Guid From32(uint uuid)
        {
            return From32(uuid, AssignedUuidBase);
        }
    }
}
