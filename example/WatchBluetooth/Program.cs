using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dandy.Devices.BluetoothLE;

namespace Dandy.Devices.Example.WatchBluetooth
{
    class Program
    {
        static void Main(string[] args)
        {
            var reset = new AutoResetEvent(false);

            // var watcher = DeviceInfo.CreateWatcher();
            // watcher.Added += (s, d) => Console.WriteLine($"Added: {d.Name} @ {d.Address} ({d.Id})");
            // watcher.Updated += (s, d) => Console.WriteLine($"Updated: {d.Id}");
            // watcher.Removed += (s, d) => Console.WriteLine($"Removed: {d.Id}");
            // watcher.EnumerationCompleted += (s, e) => Console.WriteLine("Enumeration completed");
            // watcher.Stopped += (s, e) => reset.Set();

            // note: we can't receive BLE advertisements while regular bluetooth watcher is scanning
            // (scanning == started but EnumerationCompleted has not fired)
            var adWatcher = new AdvertisementWatcher();
            adWatcher.Received += (s, e) => Console.WriteLine($"Advertisement: {PrettyPrint(e)}");
            adWatcher.Stopped += (s, e) => reset.Set();

            while (true) {
                Console.WriteLine("Press W to start device watcher, A to start BLE advertisement watcher or Q to quit");
                Action stop;
                var key = Console.ReadKey(true);
                switch (key.Key) {
                // case ConsoleKey.W:
                //     Console.WriteLine("Starting device watcher");
                //     watcher.Start();
                //     stop = () => watcher.Stop();
                //     break;
                case ConsoleKey.A:
                    Console.WriteLine("Starting BLE advertisement watcher");
                    adWatcher.Start();
                    stop = () => adWatcher.Stop();
                    break;
                case ConsoleKey.Q:
                    Console.WriteLine("Bye");
                    return;
                default:
                    continue;
                }
                Console.WriteLine("Press key to stop.");
                Console.ReadKey(true);
                Console.WriteLine("Stopping...");
                stop();
                reset.WaitOne();
                Console.WriteLine("Stopped.");
            }
        }

        static string PrettyPrint(AdvertisementReceivedEventArgs e)
        {
            var builder = new StringBuilder();
            builder.Append(e.Advertisement.LocalName);
            builder.Append(" @ ");
            builder.AppendLine(e.Address.ToString());
            foreach (var uuid in e.Advertisement.ServiceUuids) {
                builder.Append("\t");
                builder.AppendLine(uuid.ToString());
            }
            return builder.ToString();
        }
    }
}
