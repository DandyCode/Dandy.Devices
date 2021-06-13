#nullable enable

using System;
using System.Threading.Tasks;
using CoreBluetooth;

namespace Dandy.Devices.BLE.Mac
{
    public class Peripheral: IAsyncDisposable
    {
        readonly CBPeripheral peripheral;

        internal Peripheral(CBPeripheral peripheral)
        {
            this.peripheral = peripheral;
        }

        public string? Name => peripheral.Name;

        public string Id => peripheral.Identifier.AsString();

        public Task DisposeAsync()
        {
            // TODO: disconnect
            return Task.CompletedTask;
        }
    }
}
