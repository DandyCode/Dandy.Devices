# nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CoreBluetooth;
using Foundation;

namespace Dandy.Devices.BLE.Mac
{
    public class GattService
    {
        private readonly CBPeripheral peripheral;
        private readonly PeripheralDelegate @delegate;
        private readonly CBService service;

        internal GattService(CBPeripheral peripheral, PeripheralDelegate @delegate, Peripheral parent, CBService service)
        {
            this.peripheral = peripheral;
            this.@delegate = @delegate;
            Peripheral = parent;
            this.service = service;
        }

        public Peripheral Peripheral { get; }

        public Guid Uuid => Marshal.CBUuidToGuid(service.UUID);

        public async Task<IEnumerable<GattCharacteristic>> GetCharacteristicsAsync(IEnumerable<Guid>? uuids)
        {
            var cbUuids = uuids?.Select(x => CBUUID.FromString(x.ToString())).ToArray();
            var errorAwaiter = @delegate.DiscoveredCharacteristicObservable.FirstAsync(x => x.service == service).GetAwaiter();
            peripheral.DiscoverCharacteristics(cbUuids, service);
            var (_, error) = await errorAwaiter;

            if (error is not null) {
                throw new NSErrorException(error);
            }

            return service.Characteristics.Select(x => new GattCharacteristic(peripheral, @delegate, this, x));
        }

        public Task<IEnumerable<GattCharacteristic>> GetCharacteristicsAsync(params Guid[] uuids)
        {
            return GetCharacteristicsAsync((IEnumerable<Guid>)uuids);
        }
    }
}
