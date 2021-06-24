// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Threading.Tasks;
using Dandy.Devices.BLE;
using System.Reactive;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Collections.Immutable;

namespace WatchBLEAdvertisements.Mac
{
    class MainClass
    {
        static readonly Guid wedoUuidBase = new("00000000-1212-EFDE-1523-785FEABCD123");
        static readonly Guid lwp3UuidBase = new("00000000-1212-EFDE-1623-785FEABCD123");
        static readonly Guid pybricksUuidBase = new("C5F50000-8280-46DA-89F4-6D8051E4AEEF");

        static Guid WedoUuid(ushort uuid) => Uuid.From16(uuid, wedoUuidBase);
        static Guid Lwp3Uuid(ushort uuid) => Uuid.From16(uuid, lwp3UuidBase);
        static Guid PybricksUuid(ushort uuid) => Uuid.From16(uuid, pybricksUuidBase);

        static readonly Guid wedoAdvertisementDataServiceUuid = Uuid.From16(0x1523);
        static readonly Guid wedoHubServiceUuid = WedoUuid(0x1523);
        static readonly Guid wedoHubNameCharacteristicUuid = WedoUuid(0x1524);
        static readonly Guid wedoHubButtonCharacteristicUuid = WedoUuid(0x1526);
        static readonly Guid wedoHubAttachedIOCharacteristicUuid = WedoUuid(0x1527);
        static readonly Guid wedoHubLowVolatgeAlertCharacteristicUuid = WedoUuid(0x1528);
        static readonly Guid wedoHubHighCurrentAlertCharacteristicUuid = WedoUuid(0x1529);
        static readonly Guid wedoHubLowSignalAlertCharacteristicUuid = WedoUuid(0x152A);
        static readonly Guid wedoHubPowerOffCharacteristicUuid = WedoUuid(0x152B);
        static readonly Guid wedoHubPortVccCharacteristicUuid = WedoUuid(0x152C);
        static readonly Guid wedoHubBatteryTypeCharacteristicUuid = WedoUuid(0x152D);
        static readonly Guid wedoHubDisconnectCharacteristicUuid = WedoUuid(0x152E);

        static readonly Guid wedoInputServiceUuid = WedoUuid(0x4F0E);
        static readonly Guid wedoInputValueCharacteristicUuid = WedoUuid(0x1560);
        static readonly Guid wedoInputFormatCharacteristicUuid = WedoUuid(0x1561);
        static readonly Guid wedoInputCommandCharacteristicUuid = WedoUuid(0x1563);
        static readonly Guid wedoOutputCommandCharacteristicUuid = WedoUuid(0x1565);

        static readonly Guid lwp3HubServiceUuid = Lwp3Uuid(0x1623);
        static readonly Guid lwp3HubCharacteristicUuid = Lwp3Uuid(0x1624);

        static readonly Guid pybricksServiceUuid = PybricksUuid(0x0001);
        static readonly Guid pybricksCharacteristicUuid = PybricksUuid(0x0002);

        static readonly IImmutableDictionary<Guid, string> knownServices = WellKnown.Services
            .Remove(wedoHubServiceUuid) // conflicts with com.nordicsemi.service.led_and_button
            .AddRange(
                new Dictionary<Guid, string> {
                    { wedoAdvertisementDataServiceUuid, "WeDo 2.0 Advertisement Manufacturer Data" },
                    { wedoHubServiceUuid, "WeDo 2.0 Hub Service" },
                    { wedoInputServiceUuid, "WeDo 2.0 Input Service" },
                    { lwp3HubServiceUuid, "LWP3 Hub Service" },
                    { pybricksServiceUuid, "Pybricks Service" },
                }
            );

        static readonly IImmutableDictionary<Guid, string> knownCharacteristics = WellKnown.Characteristics
            .Remove(wedoHubNameCharacteristicUuid) // conflicts with com.nordicsemi.characteristic.blinky.button_state
            .AddRange(
                new Dictionary<Guid, string> {
                    { wedoHubNameCharacteristicUuid, "WeDo 2.0 Hub Name Characteristic" },
                    { wedoHubButtonCharacteristicUuid, "WeDo 2.0 Hub Button State Characteristic" },
                    { wedoHubAttachedIOCharacteristicUuid, "WeDo 2.0 Hub Attached I/O Characteristic" },
                    { wedoHubLowVolatgeAlertCharacteristicUuid, "WeDo 2.0 Hub Low Voltage Alert Characteristic" },
                    { wedoHubHighCurrentAlertCharacteristicUuid, "WeDo 2.0 Hub High Current Alert Characteristic" },
                    { wedoHubLowSignalAlertCharacteristicUuid, "WeDo 2.0 Hub Low Signal Alert Characteristic" },
                    { wedoHubPowerOffCharacteristicUuid, "WeDo 2.0 Hub Power Off Characteristic" },
                    { wedoHubPortVccCharacteristicUuid, "WeDo 2.0 Hub Port VCC Control Characteristic" },
                    { wedoHubBatteryTypeCharacteristicUuid, "WeDo 2.0 Battery Type Characteristic" },
                    { wedoHubDisconnectCharacteristicUuid, "WeDo 2.0 Hub Disconnect Characteristic" },
                    { wedoInputValueCharacteristicUuid, "WeDo 2.0 Input Value Characteristic" },
                    { wedoInputFormatCharacteristicUuid, "WeDo 2.0 Input Format Characteristic" },
                    { wedoInputCommandCharacteristicUuid, "WeDo 2.0 Input Command Characteristic" },
                    { wedoOutputCommandCharacteristicUuid, "WeDo 2.0 Output Command Characteristic" },
                    { lwp3HubCharacteristicUuid, "LWP3 Hub Characteristic" },
                    { pybricksCharacteristicUuid, "Pybricks Characteristic" },
                }
            );

        static readonly Guid[] scanUuids = new[] { wedoHubServiceUuid, lwp3HubServiceUuid, pybricksServiceUuid };

        public static async Task Main()
        {
            Console.WriteLine("Hello World!");
            await using var central = await CentralManager.NewAsync();

            var ids = new HashSet<string>();

            var advertisementObserver = Observer.Create<AdvertisementData>(
                data => {
                    try {
                        ids.Add(data.Id);
                        Console.WriteLine(data);
                    }
                    catch (Exception ex) {
                        Console.WriteLine(ex);
                        throw;
                    }
                },
                e => Console.WriteLine($"observer error: {e}"),
                () => Console.WriteLine("observer complete"));

            Console.WriteLine("scanning...");
            await using (await central.ScanAsync(advertisementObserver, scanUuids)) {
                await Task.Delay(10000);
            }

            Console.WriteLine("connecting...");
            var timeoutToken = new CancellationTokenSource(10000).Token;
            try {
                await using var peripheral = await central.ConnectAsync(ids.First(), timeoutToken);
                Console.WriteLine("connected");

                Console.WriteLine("Services:");
                foreach (var service in await peripheral.GetServicesAsync()) {
                    Console.WriteLine($"\t{service.Uuid}: {(knownServices.TryGetValue(service.Uuid, out var serviceName) ? serviceName : "<unknown>")}");

                    foreach (var characteristic in await service.GetCharacteristicsAsync()) {
                        Console.WriteLine($"\t\t{characteristic.Uuid}: {(knownCharacteristics.TryGetValue(characteristic.Uuid, out var characteristicName) ? characteristicName : "<unknown>")}");

                        foreach (var descriptor in await characteristic.GetDescriptorsAsync()) {
                            Console.WriteLine($"\t\t\t{descriptor.Uuid}: {(WellKnown.Descriptors.TryGetValue(descriptor.Uuid, out var descriptorName) ? descriptorName : "<unknown>")}");
                        }
                    }
                }
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == timeoutToken) {
                Console.WriteLine("timed out!");
            }

            Console.WriteLine("done.");
        }
    }
}
