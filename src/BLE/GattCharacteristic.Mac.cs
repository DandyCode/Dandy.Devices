// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CoreBluetooth;
using Foundation;

namespace Dandy.Devices.BLE
{
    partial class GattCharacteristic
    {
        private readonly CBPeripheral peripheral;
        private readonly PeripheralDelegate @delegate;
        private readonly CBCharacteristic characteristic;

        internal GattCharacteristic(
            CBPeripheral peripheral,
            PeripheralDelegate @delegate,
            GattService service,
            CBCharacteristic characteristic)
        {
            this.peripheral = peripheral;
            this.@delegate = @delegate;
            Service = service;
            this.characteristic = characteristic;
        }

        private partial Guid GetUuid() => Platform.CBUuidToGuid(characteristic.UUID);

        private partial GattCharacteristicProperties GetProperties() =>
            (GattCharacteristicProperties)characteristic.Properties;

        public async partial Task<IEnumerable<GattDescriptor>> GetDescriptorsAsync()
        {
            var errorAwaiter = @delegate.DiscoveredDescriptorObservable
                .FirstAsync(x => x.characteristic == characteristic).GetAwaiter();

            peripheral.DiscoverDescriptors(characteristic);
            var (_, error) = await errorAwaiter;

            if (error is not null) {
                throw new NSErrorException(error);
            }

            return characteristic.Descriptors?.Select(
                descriptor => new GattDescriptor(peripheral, @delegate, this, descriptor)
            ) ?? Enumerable.Empty<GattDescriptor>();
        }

        public async partial Task<ReadOnlyMemory<byte>> ReadAsync()
        {
            var errorAwaiter = @delegate.UpdatedCharacteristicValueObservable
                .FirstAsync(x => x.characteristic == characteristic).GetAwaiter();

            peripheral.ReadValue(characteristic);
            var (_, value, error) = await errorAwaiter;

            if (error is not null) {
                throw new NSErrorException(error);
            }

            return value.ToArray();
        }

        public async partial Task WriteAsync(ReadOnlyMemory<byte> value, bool withResponse)
        {
            using var data = NSData.FromArray(value.ToArray());

            if (withResponse) {
                var errorAwaiter = @delegate.WroteCharacteristicObservable
                    .FirstAsync(x => x.characteristic == characteristic).GetAwaiter();

                peripheral.WriteValue(data, characteristic, CBCharacteristicWriteType.WithResponse);
                var (_, error) = await errorAwaiter;

                if (error is not null) {
                    throw new NSErrorException(error);
                }
            }
            else {
                peripheral.WriteValue(data, characteristic, CBCharacteristicWriteType.WithoutResponse);
            }
        }
    }
}
