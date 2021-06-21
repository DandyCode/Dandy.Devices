// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Dandy.Devices.BLE
{
    partial class GattService
    {
        readonly GattDeviceService service;

        internal GattService(GattDeviceService service, Peripheral peripheral)
        {
            this.service = service;
            Peripheral = peripheral;
        }

        private partial Guid GetUuid() => service.Uuid;

        public async partial Task<IEnumerable<GattCharacteristic>> GetCharacteristicsAsync(IEnumerable<Guid>? uuids)
        {
            if (uuids is null || !uuids.Any()) {
                var result = await service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
                return ParseResult(result);
            }

            var characteristics = Enumerable.Empty<GattCharacteristic>();

            foreach (var uuid in uuids) {
                characteristics = characteristics.Concat(ParseResult(
                    await service.GetCharacteristicsForUuidAsync(uuid, BluetoothCacheMode.Uncached)
                ));
            }

            return characteristics;
        }

        private IEnumerable<GattCharacteristic> ParseResult(GattCharacteristicsResult result)
        {
            Platform.AssertStatus(result.Status, result.ProtocolError);
            return result.Characteristics.Select(x => new GattCharacteristic(x, this));
        }
    }
}
