// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dandy.Devices.BLE
{
    public sealed partial class GattCharacteristic
    {

        public GattService Service { get; }

        public Guid Uuid => GetUuid();

        private partial Guid GetUuid();

        public GattCharacteristicProperties Properties => GetProperties();

        private partial GattCharacteristicProperties GetProperties();

        public partial Task<IEnumerable<GattDescriptor>> GetDescriptorsAsync();

        public partial Task<ReadOnlyMemory<byte>> ReadAsync();

        public partial Task WriteAsync(ReadOnlyMemory<byte> value, bool withResponse = true);
    }
}
