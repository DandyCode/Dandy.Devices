using System;
using System.Reactive;

namespace Dandy.Devices.HID.Example.WatchHID
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var watcher = DeviceWatcher.ForPlatform()) {
                watcher.Subscribe(Observer.Create<IDevice>(
                    x => Console.WriteLine("Added {0}", x.DisplayName),
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
