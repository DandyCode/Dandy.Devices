// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Dandy.Devices.BLE
{
    internal static class Platform
    {
        private static ImmutableDictionary<byte, string> protocolErrorMap = typeof(GattProtocolError)
            .GetProperties().ToImmutableDictionary(x => (byte)x.GetValue(null)!, x => x.Name);

        internal static void AssertStatus(GattCommunicationStatus status, byte? protocolError)
        {
            switch (status) {
            case GattCommunicationStatus.Success:
                return;
            case GattCommunicationStatus.AccessDenied:
                throw new Exception("access denied");
            case GattCommunicationStatus.Unreachable:
                throw new Exception("unreachable");
            case GattCommunicationStatus.ProtocolError:
                throw new Exception($"protocol error: {protocolErrorMap[protocolError!.Value]}");
            default:
                throw new NotImplementedException("unexpected GattCommunicationStatus value");
            }
        }

        internal static Exception BluetoothErrorToException(BluetoothError error)
        {
            return new Exception($"Bluetooth error: {error}");
        }

        internal static IObservable<AdvertisementData> ApplyFilter(
            this IObservable<BluetoothLEAdvertisementReceivedEventArgs> observable,
            IEnumerable<Guid>? uuids,
            bool filterDuplicates
        )
        {
            // Built-in filtering in BluetoothLEAdvertisementWatcher is not very useful.
            // It doesn't have a way to filter duplicates and it treats scan response
            // as a separate entity, so applying any filter would elimitate scan responses.
            // So instead, we create our own.
            return Observable.Create<AdvertisementData>(observer => {
                var prevAdvDataCache = new Dictionary<ulong, BluetoothLEAdvertisementReceivedEventArgs>();
                var prevScanRspCache = new Dictionary<ulong, BluetoothLEAdvertisementReceivedEventArgs>();

                return observable.Subscribe(args => {
                    var changed = !filterDuplicates;
                    var newAdvData = args;
                    var newScanRsp = default(BluetoothLEAdvertisementReceivedEventArgs);

                    var hasPrevAdvData = prevAdvDataCache.TryGetValue(args.BluetoothAddress, out var prevAdvData);
                    var hasPrevScanRsp = prevScanRspCache.TryGetValue(args.BluetoothAddress, out var prevScanRsp);

                    // fun logic to combind scan response with non-scan-response and determine
                    // if the advertising data changed since the last received
                    switch ((
                        hasPrevAdvData ? prevAdvData!.AdvertisementType == BluetoothLEAdvertisementType.ScanResponse : default(bool?),
                        hasPrevScanRsp ? prevScanRsp!.AdvertisementType == BluetoothLEAdvertisementType.ScanResponse : default(bool?),
                        args.AdvertisementType == BluetoothLEAdvertisementType.ScanResponse
                    )) {
                    case (null, null, false):
                        // new adv data, no prev adv data, no prev scan rsp
                        changed = true;
                        break;
                    case (null, null, true):
                        // new scan rsp, no prev adv data, no prev scan rsp
                        // new scan rsp goes in adv data slot since there is no new or prev adv data
                        changed = true;
                        break;
                    case (false, null, false):
                        // new adv data replaces old adv data, no prev scan rsp
                        if (filterDuplicates) {
                            changed = !Platform.AdvertisementEqual(
                                newAdvData.Advertisement, prevAdvData!.Advertisement);
                        }
                        break;
                    case (false, null, true):
                        // new scan rsp added to prev adv data, no prev scan rsp
                        newAdvData = prevAdvData!;
                        newScanRsp = args;
                        changed = true;
                        break;
                    case (true, null, false):
                        // new adv data added to prev scan rsp, no prev adv data
                        // prev scan rsp was in adv data slot since there was no prev adv data
                        newScanRsp = prevAdvData!;
                        changed = true;
                        break;
                    case (true, null, true):
                        // new scan rsp replaces prev scan rsp, no prev adv data
                        // prev scan rsp was in adv data slot since there was no prev adv data
                        // new scan rsp goes in adv data slot since there is no new or prev adv data
                        if (filterDuplicates) {
                            changed = !Platform.AdvertisementEqual(
                                newAdvData.Advertisement, prevAdvData!.Advertisement);
                        }
                        break;
                    case (false, true, false):
                        // new adv data replaces old adv data, keep prev scan rsp
                        newScanRsp = prevScanRsp!;
                        if (filterDuplicates) {
                            changed = !Platform.AdvertisementEqual(
                                newAdvData.Advertisement, prevAdvData!.Advertisement);
                        }
                        break;
                    case (false, true, true):
                        // new scan rsp replaces prev scan rsp, keep prev adv data
                        newAdvData = prevAdvData!;
                        newScanRsp = prevScanRsp!;
                        if (filterDuplicates) {
                            changed = !Platform.AdvertisementEqual(
                                newScanRsp.Advertisement, prevScanRsp!.Advertisement);
                        }
                        break;
                    }

                    prevAdvDataCache[args.BluetoothAddress] = newAdvData;

                    if (newScanRsp is not null) {
                        prevScanRspCache[args.BluetoothAddress] = newScanRsp;
                    }

                    if (changed) {
                        var ad = new AdvertisementData(newAdvData, newScanRsp?.Advertisement);
                        if (uuids is not null && ad.ServiceUuids.Any(x => uuids.Contains(x))) {
                            observer.OnNext(ad);
                        }
                    }
                });
            });
        }

        /// <summary>
        /// Tests if two advertisement contain the same data.
        /// </summary>
        private static bool AdvertisementEqual(BluetoothLEAdvertisement one, BluetoothLEAdvertisement two)
        {
            if (!one.DataSections.Select(x => x.Data.Length).SequenceEqual(two.DataSections.Select(x => x.Data.Length))) {
                return false;
            }

            return one.DataSections.Zip(two.DataSections).All(x => x.First.Data.ToArray().SequenceEqual(x.Second.Data.ToArray()));
        }
    }
}
