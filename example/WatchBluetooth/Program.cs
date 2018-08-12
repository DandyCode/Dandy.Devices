using System;
using System.Threading;
using System.Threading.Tasks;
using Dandy.Devices.Bluetooth;

namespace Dandy.Devices.Example.WatchBluetooth
{
    class Program
    {
        static void Main(string[] args)
        {
            var reset = new AutoResetEvent(false);

            var watcher = DeviceInfo.CreateWatcher();
            watcher.Added += (s, d) => Console.WriteLine($"Added: {d.Name} @ {d.Address} ({d.Id})");
            // watcher.Updated += (s, d) => Console.WriteLine($"Updated: {d.Id}");
            watcher.Removed += (s, d) => Console.WriteLine($"Removed: {d.Id}");
            watcher.EnumerationCompleted += (s, e) => Console.WriteLine("Enumeration completed");
            watcher.Stopped += (s, e) => {
                Console.WriteLine("Stopped");
                reset.Set();
            };
            watcher.Start();

            Console.WriteLine("Press key to exit.");
            Console.ReadKey();
            Console.WriteLine("Stopping...");
            watcher.Stop();

            reset.WaitOne();
            Console.WriteLine("Bye");
        }
    }
}
