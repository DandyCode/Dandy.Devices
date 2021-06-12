using System;
using System.Threading.Tasks;
using Dandy.Devices.BLE;

namespace WatchBLEAdvertisements
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var manager = await CentralManager.NewAsync();
            Console.WriteLine("done.");
        }
    }
}
