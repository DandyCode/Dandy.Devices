// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dandy.Devices.BLE
{
    public sealed partial class GattService
    {
        public Peripheral Peripheral { get; }

        public Guid Uuid => GetUuid();

        private partial Guid GetUuid();

        public partial Task<IEnumerable<GattCharacteristic>> GetCharacteristicsAsync(IEnumerable<Guid>? uuids);

        public Task<IEnumerable<GattCharacteristic>> GetCharacteristicsAsync(params Guid[] uuids)
        {
            return GetCharacteristicsAsync((IEnumerable<Guid>)uuids);
        }
    }
}
