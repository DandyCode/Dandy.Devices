// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dandy.Devices.BLE
{
    public sealed partial class Peripheral
    {
        public string Id => GetId();

        private partial string GetId();

        public string? Name => GetName();

        private partial string? GetName();

        public partial Task<IEnumerable<GattService>> GetServicesAsync(IEnumerable<Guid>? uuids);

        public Task<IEnumerable<GattService>> GetServicesAsync(params Guid[] uuids)
        {
            return GetServicesAsync((IEnumerable<Guid>?)uuids);
        }
    }
}
