// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Collections.Immutable;
using CoreBluetooth;
using Foundation;

using static System.Buffers.Binary.BinaryPrimitives;
using CBAdvertisementData = CoreBluetooth.AdvertisementData;

namespace Dandy.Devices.BLE
{
    partial class AdvertisementData
    {
        private readonly CBPeripheral peripheral;
        private readonly CBAdvertisementData advertisementData;
        private readonly NSNumber rssi;

        internal AdvertisementData(CBPeripheral peripheral, NSDictionary advertisementData, NSNumber rssi)
        {
            this.peripheral = peripheral;
            this.advertisementData = new(advertisementData);
            this.rssi = rssi;
        }

        private partial string GetId() => peripheral.Identifier.AsString();

        private partial string? GetLocalName() => advertisementData.LocalName;

        private partial IImmutableDictionary<ushort, ReadOnlyMemory<byte>> GetManufacturerData() =>
            ParseManufacturerData(advertisementData.ManufacturerData);

        private partial IImmutableDictionary<Guid, ReadOnlyMemory<byte>> GetServiceData() =>
            // https://github.com/xamarin/xamarin-macios/issues/11917
            // The strong dictionary is returning null even when there is
            // service data, so we have added a workaround to get the service
            // data from the underlying dictionary.
            ParseServiceData(advertisementData.ServiceData ?? WorkaroundNullServiceData(advertisementData));

        private partial IImmutableSet<Guid> GetServiceUuids() =>
            ParseServiceUuids(advertisementData.ServiceUuids, advertisementData.OverflowServiceUuids);

        private partial short? GetTxPower() => advertisementData.TxPowerLevel?.Int16Value;

        private partial short GetRssi() => rssi.Int16Value;

        private static IImmutableDictionary<ushort, ReadOnlyMemory<byte>>
            ParseManufacturerData(NSData? manufacturerData)
        {
            var mfgData = ImmutableDictionary.Create<ushort, ReadOnlyMemory<byte>>();

            if (manufacturerData is not null) {
                var rawData = new ReadOnlyMemory<byte>(manufacturerData.ToArray());
                var cid = ReadUInt16LittleEndian(rawData.Span);
                var data = rawData[2..];
                mfgData = mfgData.Add(cid, data);
            }

            return mfgData;
        }

        private static IImmutableDictionary<Guid, ReadOnlyMemory<byte>>
            ParseServiceData(NSDictionary? serviceData)
        {
            var builder = ImmutableDictionary.CreateBuilder<Guid, ReadOnlyMemory<byte>>();

            if (serviceData is not null) {
                foreach (var item in serviceData) {
                    var guid = Platform.CBUuidToGuid((CBUUID)item.Key);
                    var nsdata = (NSData)item.Value;
                    var data = new ReadOnlyMemory<byte>(nsdata.ToArray());
                    builder.Add(guid, data);
                }
            }

            return builder.ToImmutable();
        }

        private static NSDictionary? WorkaroundNullServiceData(CBAdvertisementData advertisingData)
        {
            if (advertisingData.Dictionary.TryGetValue(CBAdvertisement.DataServiceDataKey, out var serviceData)) {
                return (NSDictionary)serviceData;
            }

            return null;
        }

        private static IImmutableSet<Guid> ParseServiceUuids(CBUUID[]? serviceUuids, CBUUID[]? overflowServiceUuids)
        {
            var builder = ImmutableHashSet.CreateBuilder<Guid>();

            if (serviceUuids is not null) {
                foreach (var s in serviceUuids) {
                    builder.Add(new Guid(s.Uuid));
                }
            }

            if (overflowServiceUuids is not null) {
                foreach (var s in overflowServiceUuids) {
                    builder.Add(new Guid(s.Uuid));
                }
            }

            return builder.ToImmutable();
        }
    }
}
