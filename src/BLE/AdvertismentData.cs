// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Dandy.Devices.BLE
{
    /// <summary>
    /// Advertisement data received from remote device.
    /// </summary>
    public sealed partial class AdvertisementData
    {
        /// <summary>
        /// Gets a unique identifier for the device.
        /// </summary>
        /// <remarks>
        /// This is used to connect to the device. It is should not be displayed
        /// to the user (other than for debugging purposes).
        /// </remarks>
        public string Id => GetId();

        private partial string GetId();

        /// <summary>
        /// Gets the local name from the advertising data, if it is present.
        /// </summary>
        public string? LocalName => GetLocalName();

        private partial string? GetLocalName();

        /// <summary>
        /// Gets a dictionary of manufacturer-specific data from the advertising data.
        /// </summary>
        public IImmutableDictionary<ushort, ReadOnlyMemory<byte>> ManufacturerData =>
            GetManufacturerData();

        private partial IImmutableDictionary<ushort, ReadOnlyMemory<byte>> GetManufacturerData();

        /// <summary>
        /// Gets a dictionary of service data from the advertising data.
        /// </summary>
        public IImmutableDictionary<Guid, ReadOnlyMemory<byte>> ServiceData =>
            GetServiceData();

        private partial IImmutableDictionary<Guid, ReadOnlyMemory<byte>> GetServiceData();

        /// <summary>
        /// Gets a list of service UUIDs from the advertising data.
        /// </summary>
        public IImmutableSet<Guid> ServiceUuids => GetServiceUuids();

        private partial IImmutableSet<Guid> GetServiceUuids();

        /// <summary>
        /// Gets the transmit power from the advertising data, if present.
        /// </summary>
        public short? TxPower => GetTxPower();

        private partial short? GetTxPower();

        /// <summary>
        /// Gets the radio signal strength indication from the advertising data.
        /// </summary>
        public short Rssi => GetRssi();

        private partial short GetRssi();

        /// <summary>
        /// Gets a string describing the advertising data.
        /// </summary>
        /// <returns>The string.</returns>
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
