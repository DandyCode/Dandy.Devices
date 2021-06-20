// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Threading.Tasks;

namespace Dandy.Devices.BLE
{
    public sealed partial class GattDescriptor
    {
        public GattCharacteristic Characteristic { get; }

        public Guid Uuid => GetUuid();

        private partial Guid GetUuid();

        public partial Task<ReadOnlyMemory<byte>> ReadAsync();

        public partial Task WriteAsync(ReadOnlyMemory<byte> value);
    }
}
