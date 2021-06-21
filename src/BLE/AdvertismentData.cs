// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Dandy.Devices.BLE
{
    public partial class AdvertisementData
    {
        public string Id => GetId();

        private partial string GetId();

        public string? LocalName => GetLocalName();

        private partial string? GetLocalName();

        public IImmutableDictionary<ushort, ReadOnlyMemory<byte>> ManufacturerData =>
            GetManufacturerData();

        private partial IImmutableDictionary<ushort, ReadOnlyMemory<byte>> GetManufacturerData();

        public IImmutableDictionary<Guid, ReadOnlyMemory<byte>> ServiceData =>
            GetServiceData();

        private partial IImmutableDictionary<Guid, ReadOnlyMemory<byte>> GetServiceData();

        public IImmutableSet<Guid> ServiceUuids => GetServiceUuids();

        private partial IImmutableSet<Guid> GetServiceUuids();

        public short? TxPower => GetTxPower();

        private partial short? GetTxPower();

        public short Rssi => GetRssi();

        private partial short GetRssi();

        public override string ToString()
        {
            var items = new List<string> {
                $"ID: {Id}"
            };

            if (LocalName is not null) {
                items.Add($"LocalName: {LocalName}");
            }

            if (ManufacturerData.Any()) {
                items.Add($"ManufacturerData: {string.Join(", ", ManufacturerData.Select(x => $"{x.Key}: {BitConverter.ToString(x.Value.ToArray())}"))}");
            }

            if (ServiceData.Any()) {
                items.Add($"ServiceData: {string.Join(", ", ServiceData.Select(x => $"{x.Key}: {BitConverter.ToString(x.Value.ToArray())}"))}");
            }

            if (ServiceUuids.Any()) {
                items.Add($"ServiceUuids: {string.Join(", ", ServiceUuids)}");
            }

            if (TxPower is not null) {
                items.Add($"TxPower: {TxPower}");
            }

            items.Add($"RSSI: {Rssi}");

            return string.Join("; ", items);
        }
    }
}
