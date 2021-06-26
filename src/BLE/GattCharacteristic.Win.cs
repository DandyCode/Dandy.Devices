// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

// https://github.com/dotnet/roslyn/issues/54103
#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

using GattDeviceCharacteristic = Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristic;

namespace Dandy.Devices.BLE
{
    partial class GattCharacteristic
    {
        readonly GattDeviceCharacteristic characteristic;

        internal GattCharacteristic(GattDeviceCharacteristic characteristic, GattService service)
        {
            this.characteristic = characteristic;
            Service = service;
        }

        private partial Guid GetUuid() => characteristic.Uuid;

        private partial GattCharacteristicProperties GetProperties() =>
            (GattCharacteristicProperties)characteristic.CharacteristicProperties;

        public async partial Task<IEnumerable<GattDescriptor>> GetDescriptorsAsync() =>
            ParseResult(await characteristic.GetDescriptorsAsync(BluetoothCacheMode.Uncached));

        private IEnumerable<GattDescriptor> ParseResult(GattDescriptorsResult result)
        {
            Platform.AssertStatus(result.Status, result.ProtocolError);
            return result.Descriptors.Select(x => new GattDescriptor(x, this));
        }

        public async partial Task<ReadOnlyMemory<byte>> ReadAsync()
        {
            var result = await characteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
            Platform.AssertStatus(result.Status, result.ProtocolError);
            return result.Value.ToArray();
        }

        public async partial Task WriteAsync(ReadOnlyMemory<byte> value, bool withResponse)
        {
            var buffer = value.ToArray().AsBuffer();
            var option = withResponse ? GattWriteOption.WriteWithResponse : GattWriteOption.WriteWithoutResponse;
            var result = await characteristic.WriteValueWithResultAsync(buffer, option);
            Platform.AssertStatus(result.Status, result.ProtocolError);
        }
    }
}
