#nullable enable

using System;
using System.Reflection;
using System.Threading.Tasks;
using Dandy.Devices.BLE.Mac;
using ObjCRuntime;
using System.Reactive;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace WatchBLEAdvertisements.Mac
{
    class MainClass
    {
        static readonly Guid wedoUuidBase = new("00000000-1212-EFDE-1523-785FEABCD123");
        static readonly Guid lwp3UuidBase = new("00000000-1212-EFDE-1623-785FEABCD123");
        static readonly Guid pybricksUuidBase = new("C5F50000-8280-46DA-89F4-6D8051E4AEEF");

        static Guid wedoUuid(ushort uuid) => Uuid.From16(uuid, wedoUuidBase);
        static Guid lpw3Uuid(ushort uuid) => Uuid.From16(uuid, lwp3UuidBase);
        static Guid pybricksUuid(ushort uuid) => Uuid.From16(uuid, pybricksUuidBase);

        static readonly Guid wedoHubServiceUuid = wedoUuid(0x1523);
        static readonly Guid lwp3HubServiceUuid = lpw3Uuid(0x1623);
        static readonly Guid pybricksServiceUuid = pybricksUuid(0x0001);

        static readonly Guid[] scanUuids = new[] { wedoHubServiceUuid, lwp3HubServiceUuid, pybricksServiceUuid };

        public static async Task Main()
        {
            // HACK: can't use NSApplication.Init() because it messes with the syncronization context
            typeof(Runtime).GetMethod("EnsureInitialized", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
            typeof(Runtime).GetMethod("RegisterAssemblies", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);

            Console.WriteLine("Hello World!");
            await using var central = await CentralManager.NewAsync();

            var ids = new HashSet<string>();

            var advertisementObserver = Observer.Create<AdvertisementData>(
                data => {
                    ids.Add(data.Id);
                    Console.WriteLine(data);
                },
                () => Console.WriteLine("observer complete"));

            Console.WriteLine("scanning...");
            await using (await central.ScanAsync(advertisementObserver, scanUuids)) {
                await Task.Delay(10000);
            }

            Console.WriteLine("connecting...");
            await using (var peripherial = await central.ConnectAsync(ids.First(), new CancellationTokenSource(10000).Token)) {
                Console.WriteLine("connected");
                foreach (var p in central.GetConnectedPeripherals()) {
                    Console.Write("Connected: ");
                    Console.WriteLine(p);
                }
            }

            Console.WriteLine("done.");
        }
    }
}

namespace System
{
    interface IAsyncDisposable
    {
        Task DisposeAsync();
    }
}
