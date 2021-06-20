// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CoreBluetooth;
using Foundation;

namespace Dandy.Devices.BLE
{
    partial class GattDescriptor
    {
        private readonly CBPeripheral peripheral;
        private readonly PeripheralDelegate @delegate;
        private readonly CBDescriptor descriptor;

        internal GattDescriptor(
            CBPeripheral peripheral,
            PeripheralDelegate @delegate,
            GattCharacteristic characteristic,
            CBDescriptor descriptor)
        {
            this.peripheral = peripheral;
            this.@delegate = @delegate;
            Characteristic = characteristic;
            this.descriptor = descriptor;
        }

        private partial Guid GetUuid() => Platform.CBUuidToGuid(descriptor.UUID);

        public async partial Task<ReadOnlyMemory<byte>> ReadAsync()
        {
            var errorAwaiter = @delegate.UpdatedDescriptorValueObservable
                .FirstAsync(x => x.descriptor == descriptor).GetAwaiter();
            peripheral.ReadValue(descriptor);
            var (_, value, error) = await errorAwaiter;

            if (error is not null) {
                throw new NSErrorException(error);
            }

            return ValueToMemory(value);
        }

        public async partial Task WriteAsync(ReadOnlyMemory<byte> value)
        {
            var errorAwaiter = @delegate.WroteDescriptorObservable
                .FirstAsync(x => x.descriptor == descriptor).GetAwaiter();

            peripheral.WriteValue(Platform.MemoryToNSData(value), descriptor);

            var (_, error) = await errorAwaiter;

            if (error is not null) {
                throw new NSErrorException(error);
            }
        }

        private static ReadOnlyMemory<byte> ValueToMemory(NSObject? value)
        {
            return value switch {
                NSData data => data.ToArray(),
                NSString str => str.Encode(NSStringEncoding.UTF8).ToArray(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
