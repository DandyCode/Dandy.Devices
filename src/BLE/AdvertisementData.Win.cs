// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth.Advertisement;

using static System.Buffers.Binary.BinaryPrimitives;


namespace Dandy.Devices.BLE
{
    partial class AdvertisementData
    {
        readonly BluetoothLEAdvertisementReceivedEventArgs args;
        readonly BluetoothLEAdvertisement? scanResponse;

        internal AdvertisementData(
            BluetoothLEAdvertisementReceivedEventArgs args,
            BluetoothLEAdvertisement? scanResponse = null)
        {
            this.args = args;
            this.scanResponse = scanResponse;
        }

        private partial string GetId() => args.BluetoothAddress.ToString();

        // have to check for data sections, otherwise LocalData would return an
        // empty string instead of null.
        private partial string? GetLocalName() => args.Advertisement.DataSections
            .Concat(scanResponse?.DataSections ?? Enumerable.Empty<BluetoothLEAdvertisementDataSection>())
            .Any(
                x => x.DataType == BluetoothLEAdvertisementDataTypes.CompleteLocalName
                || x.DataType == BluetoothLEAdvertisementDataTypes.ShortenedLocalName
            ) ? scanResponse?.LocalName ?? args.Advertisement.LocalName : null;

        private partial IImmutableDictionary<ushort, ReadOnlyMemory<byte>> GetManufacturerData() =>
            args.Advertisement.ManufacturerData
            .Concat(scanResponse?.ManufacturerData ?? Enumerable.Empty<BluetoothLEManufacturerData>())
            .ToImmutableDictionary(
                x => x.CompanyId, x => (ReadOnlyMemory<byte>)x.Data.ToArray());

        private partial IImmutableDictionary<Guid, ReadOnlyMemory<byte>> GetServiceData() =>
            args.Advertisement.GetSectionsByType(
                BluetoothLEAdvertisementDataTypes.ServiceData16BitUuids
            ).Concat(scanResponse?.GetSectionsByType(
                BluetoothLEAdvertisementDataTypes.ServiceData16BitUuids
            ) ?? Enumerable.Empty<BluetoothLEAdvertisementDataSection>())
            .Select(x => (ReadOnlyMemory<byte>)x.Data.ToArray())
            .ToImmutableDictionary(
                x => Uuid.From16(ReadUInt16LittleEndian(x.Slice(0, 2).Span)),
                x => x.Slice(2)
            ).AddRange(
                args.Advertisement.GetSectionsByType(
                    BluetoothLEAdvertisementDataTypes.ServiceData32BitUuids
                ).Concat(scanResponse?.GetSectionsByType(
                    BluetoothLEAdvertisementDataTypes.ServiceData32BitUuids
                ) ?? Enumerable.Empty<BluetoothLEAdvertisementDataSection>())
                .Select(x => (ReadOnlyMemory<byte>)x.Data.ToArray())
                .ToDictionary(
                    x => Uuid.From32(ReadUInt32LittleEndian(x.Slice(0, 4).Span)),
                    x => x.Slice(4)
                )
            ).AddRange(
                args.Advertisement.GetSectionsByType(
                    BluetoothLEAdvertisementDataTypes.ServiceData128BitUuids
                ).Concat(scanResponse?.GetSectionsByType(
                    BluetoothLEAdvertisementDataTypes.ServiceData128BitUuids
                ) ?? Enumerable.Empty<BluetoothLEAdvertisementDataSection>())
                .Select(x => (ReadOnlyMemory<byte>)x.Data.ToArray())
                .ToDictionary(
                    x => new Guid(x.Slice(0, 16).ToArray()),
                    x => x.Slice(16)
                )
            );

        private partial IImmutableSet<Guid> GetServiceUuids() =>
            args.Advertisement.ServiceUuids
            .Concat(scanResponse?.ServiceUuids ?? Enumerable.Empty<Guid>())
            .ToImmutableHashSet();

        private partial short? GetTxPower() => args.Advertisement.GetSectionsByType(
            BluetoothLEAdvertisementDataTypes.TxPowerLevel
        ).Select(x => (sbyte)x.Data.GetByte(0))
        .SingleOrDefault();

        private partial short GetRssi() => args.RawSignalStrengthInDBm;
    }
}
