#nullable enable

using System;
using CoreBluetooth;

namespace Dandy.Devices.BLE.Mac
{
    public class Peripheral
    {
        readonly CBCentralManager central;
        readonly CentralManagerDelegate centralDelegate;
        readonly CBPeripheral peripheral;

        internal Peripheral(CBCentralManager central, CentralManagerDelegate centralDelegate, CBPeripheral peripheral)
        {
            this.central = central;
            this.centralDelegate = centralDelegate;
            this.peripheral = peripheral;
        }

        public string? Name => peripheral.Name;

        public string Id => peripheral.Identifier.AsString();
    }
}
