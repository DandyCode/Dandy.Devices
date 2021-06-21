// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

using GattDeviceDescriptor = Windows.Devices.Bluetooth.GenericAttributeProfile.GattDescriptor;

namespace Dandy.Devices.BLE
{
    partial class GattDescriptor
    {
        readonly GattDeviceDescriptor descriptor;

        internal GattDescriptor(GattDeviceDescriptor descriptor, GattCharacteristic characteristic)
        {
            this.descriptor = descriptor;
            Characteristic = characteristic;
        }

        private partial Guid GetUuid() => descriptor.Uuid;

        public async partial Task<ReadOnlyMemory<byte>> ReadAsync()
        {
            var result = await descriptor.ReadValueAsync(BluetoothCacheMode.Uncached);
            Platform.AssertStatus(result.Status, result.ProtocolError);
            return result.Value.ToArray();
        }

        public async partial Task WriteAsync(ReadOnlyMemory<byte> value)
        {
            var buffer = value.ToArray().AsBuffer();
            var result = await descriptor.WriteValueWithResultAsync(buffer);
            Platform.AssertStatus(result.Status, result.ProtocolError);
        }
    }
}
