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

        private partial IImmutableDictionary<ushort, ImmutableArray<byte>>? GetManufacturerData() =>
            ParseManufacturerData(advertisementData.ManufacturerData);

        private partial IImmutableDictionary<Guid, ImmutableArray<byte>>? GetServiceData() =>
            // https://github.com/xamarin/xamarin-macios/issues/11917
            // The strong dictionary is returning null even when there is
            // service data, so we have added a workaround to get the service
            // data from the underlying dictionary.
            ParseServiceData(advertisementData.ServiceData ?? WorkaroundNullServiceData(advertisementData));

        private partial IImmutableSet<Guid>? GetServiceUuids() =>
            ParseServiceUuids(advertisementData.ServiceUuids, advertisementData.OverflowServiceUuids);

        private partial short? GetTxPower() => advertisementData.TxPowerLevel?.Int16Value;

        private partial short GetRssi() => rssi.Int16Value;

        private static unsafe IImmutableDictionary<ushort, ImmutableArray<byte>>?
            ParseManufacturerData(NSData? manufacturerData)
        {
            if (manufacturerData is null) {
                return null;
            }

            var span = new ReadOnlySpan<byte>((void*)manufacturerData.Bytes, (int)manufacturerData.Length);
            var cid = ReadUInt16LittleEndian(span);
            var data = ImmutableArrayFromSpan(span.Slice(2));

            return ImmutableDictionary.Create<ushort, ImmutableArray<byte>>().Add(cid, data);
        }

        private static unsafe IImmutableDictionary<Guid, ImmutableArray<byte>>?
            ParseServiceData(NSDictionary? serviceData)
        {
            if (serviceData is null) {
                return null;
            }

            var builder = ImmutableDictionary.CreateBuilder<Guid, ImmutableArray<byte>>();

            foreach (var item in serviceData) {
                var guid = Platform.CBUuidToGuid((CBUUID)item.Key);
                var nsdata = (NSData)item.Value;
                var span = new ReadOnlySpan<byte>((void*)nsdata.Bytes, (int)nsdata.Length);
                var data = ImmutableArrayFromSpan(span);
                builder.Add(guid, data);
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

        private static IImmutableSet<Guid>? ParseServiceUuids(CBUUID[]? serviceUuids, CBUUID[]? overflowServiceUuids)
        {
            if (serviceUuids is null && overflowServiceUuids is null) {
                return null;
            }

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

        private static ImmutableArray<T> ImmutableArrayFromSpan<T>(ReadOnlySpan<T> span)
        {
            // Maybe a Span API some day: https://github.com/dotnet/runtime/issues/22928
            var builder = ImmutableArray.CreateBuilder<T>(span.Length);
            foreach (var i in span) {
                builder.Add(i);
            }
            return builder.MoveToImmutable();
        }
    }
}
