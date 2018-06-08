using System;
using System.Reactive;

namespace Dandy.Devices.HID.Example.WatchHID
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var watcher = DeviceWatcher.ForPlatform()) {
                watcher.Subscribe(Observer.Create<Device>(
                    x => Console.WriteLine("Added {0}", x.Id),
                    x => Console.WriteLine("Error: {0}", x.Message),
                    () => Console.WriteLine("Complete")
                ));
                using (watcher.Connect()) {
                    Console.ReadLine();
                }
            }
            System.Threading.Thread.Sleep(1000);
        }
    }
}
