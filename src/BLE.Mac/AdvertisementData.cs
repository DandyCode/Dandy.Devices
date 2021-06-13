#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CoreBluetooth;
using Foundation;

using static System.Buffers.Binary.BinaryPrimitives;
using CBAdvertisementData = CoreBluetooth.AdvertisementData;

namespace Dandy.Devices.BLE.Mac
{
    public class AdvertisementData
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

        public string Id => peripheral.Identifier.AsString();

        public string? LocalName => advertisementData.LocalName;

        public IImmutableDictionary<ushort, ImmutableArray<byte>>? ManufacturerData =>
            ParseManufacturerData(advertisementData.ManufacturerData);

        public IImmutableDictionary<Guid, ImmutableArray<byte>>? ServiceData =>
            ParseServiceData(advertisementData.ServiceData);

        public IImmutableSet<Guid>? ServiceUuids =>
            ParseServiceUuids(advertisementData.ServiceUuids, advertisementData.OverflowServiceUuids);

        public short? TxPower => advertisementData.TxPowerLevel?.Int16Value;

        public short Rssi => rssi.Int16Value;

        public override string ToString()
        {
            var items = new List<string>();

            items.Add($"ID: {Id}");

            if (LocalName is not null) {
                items.Add($"LocalName: {LocalName}");
            }

            if (ManufacturerData is not null) {
                items.Add($"ManufacturerData: {string.Join(", ", ManufacturerData.Select(x => $"{x.Key}: {BitConverter.ToString(x.Value.ToArray())}"))}");
            }

            if (ServiceData is not null) {
                items.Add($"ServiceData: {string.Join(", ", ServiceData.Select(x => $"{x.Key}: {BitConverter.ToString(x.Value.ToArray())}"))}");
            }

            if (ServiceUuids is not null) {
                items.Add($"ServiceUuids: {string.Join(", ", ServiceUuids)}");
            }

            if (TxPower is not null) {
                items.Add($"TxPower: {TxPower}");
            }

            items.Add($"RSSI: {Rssi}");

            return string.Join("; ", items);
        }

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
                var guid = new Guid(((CBUUID)item.Key).Uuid);
                var nsdata = (NSData)item.Value;
                var span = new ReadOnlySpan<byte>((void*)nsdata.Bytes, (int)nsdata.Length);
                var data = ImmutableArrayFromSpan(span);
                builder.Add(guid, data);
            }

            return builder.ToImmutable();
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
