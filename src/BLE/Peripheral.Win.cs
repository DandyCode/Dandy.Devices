// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

// https://github.com/dotnet/roslyn/issues/54103
#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Dandy.Devices.BLE
{
    partial class Peripheral
    {
        readonly BluetoothLEDevice device;
        readonly GattSession session;

        internal Peripheral(BluetoothLEDevice device, GattSession session)
        {
            this.device = device;
            this.session = session;
        }

        private partial string GetId() => device.BluetoothAddress.ToString();

        private partial string? GetName() => device.Name;

        public async partial Task<IEnumerable<GattService>> GetServicesAsync(IEnumerable<Guid>? uuids)
        {
            if (uuids is null || !uuids.Any()) {
                var result = await device.GetGattServicesAsync(BluetoothCacheMode.Uncached);
                return ParseResult(result);
            }

            var services = Enumerable.Empty<GattService>();

            foreach (var uuid in uuids) {
                services = services.Concat(ParseResult(
                    await device.GetGattServicesForUuidAsync(uuid, BluetoothCacheMode.Uncached)
                ));
            }

            return services;
        }

        private IEnumerable<GattService> ParseResult(GattDeviceServicesResult result)
        {
            Platform.AssertStatus(result.Status, result.ProtocolError);
            return result.Services.Select(x => new GattService(x, this));
        }

        public partial ValueTask DisposeAsync()
        {
            session.Dispose();
            device.Dispose();
            // TODO: wait for disconnect
            return ValueTask.CompletedTask;
        }
    }
}
