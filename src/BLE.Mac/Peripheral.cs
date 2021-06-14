#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CoreBluetooth;
using Foundation;

namespace Dandy.Devices.BLE.Mac
{
    public class Peripheral : IAsyncDisposable
    {
        readonly CBCentralManager central;
        readonly CentralManagerDelegate centralDelegate;
        private readonly PeripheralDelegate @delegate;
        private readonly CBPeripheral peripheral;

        internal Peripheral(CBCentralManager central, CentralManagerDelegate centralDelegate, CBPeripheral peripheral)
        {
            this.central = central;
            this.centralDelegate = centralDelegate;
            @delegate = new PeripheralDelegate();
            this.peripheral = peripheral;
            peripheral.Delegate = @delegate;
        }

        public string? Name => peripheral.Name;

        public string Id => peripheral.Identifier.AsString();

        public async Task DisposeAsync()
        {
            var errorAwaiter = centralDelegate.DisconnectedPeripheralObservable
                .FirstAsync(x => x.peripheral.Identifier == peripheral.Identifier)
                .GetAwaiter();

            if (!peripheral.IsConnected) {
                return;
            }

            central.CancelPeripheralConnection(peripheral);

            var (_, error) = await errorAwaiter;
        }

        public async Task<IEnumerable<GattService>> GetServicesAsync(IEnumerable<Guid>? uuids = null)
        {
            var cbUuids = uuids?.Select(x => CBUUID.FromString(x.ToString())).ToArray();
            var errorAwaiter = @delegate.DiscoveredServiceObservable.FirstAsync().GetAwaiter();
            peripheral.DiscoverServices(cbUuids);
            var error = await errorAwaiter;

            if (error is not null) {
                throw new NSErrorException(error);
            }

            return peripheral.Services.Select(x => new GattService(peripheral, @delegate, this, x));
        }

        public Task<IEnumerable<GattService>> GetServicesAsync(params Guid[] uuids)
        {
            return GetServicesAsync((IEnumerable<Guid>?)uuids);
        }
    }

    internal class PeripheralDelegate : CBPeripheralDelegate
    {
        private readonly Subject<NSError?> discoveredServiceSubject = new();
        private readonly Subject<(CBService service, NSError? error)> discoveredCharacteristicSubject = new();
        private readonly Subject<(CBCharacteristic characteristic, NSError? error)> discoveredDescriptorSubject = new();
        private readonly Subject<(CBCharacteristic characteristic, NSData? value, NSError? error)> updatedCharacteristicValueSubject = new();
        private readonly Subject<(CBDescriptor descriptor, NSObject? value, NSError? error)> updatedDescriptorValueSubject = new();
        private readonly Subject<(CBCharacteristic characteristic, NSError? error)> wroteCharacteristicSubject = new();
        private readonly Subject<(CBDescriptor descriptor, NSError? error)> wroteDescriptorSubject = new();
        private readonly Subject<(CBCharacteristic characteristic, bool isNotifying, NSError? error)> updatedNotificationState = new();
        private readonly Subject<(NSNumber rssi, NSError? error)> updatedRssiSubject = new();

        public IObservable<NSError?> DiscoveredServiceObservable => discoveredServiceSubject.AsObservable();
        public IObservable<(CBService service, NSError? error)> DiscoveredCharacteristicObservable => discoveredCharacteristicSubject.AsObservable();
        public IObservable<(CBCharacteristic characteristic, NSError? error)> DiscoveredDescriptorObservable => discoveredDescriptorSubject.AsObservable();
        public IObservable<(CBCharacteristic characteristic, NSData? value, NSError? error)> UpdatedCharacteristicValueObservable => updatedCharacteristicValueSubject.AsObservable();
        public IObservable<(CBDescriptor descriptor, NSObject? value, NSError? error)> UpdatedDescriptorValueObservable => updatedDescriptorValueSubject.AsObservable();
        public IObservable<(CBCharacteristic characteristic, NSError? error)> WroteCharacteristicObservable => wroteCharacteristicSubject.AsObservable();
        public IObservable<(CBDescriptor descriptor, NSError? error)> WroteDescriptorObservable => wroteDescriptorSubject.AsObservable();
        public IObservable<(CBCharacteristic characteristic, bool isNotifying, NSError? error)> UpdatedNotificationStateObservable => updatedNotificationState.AsObservable();
        public IObservable<(NSNumber rssi, NSError? error)> UpdatedRssiObservable => updatedRssiSubject.AsObservable();

        public override void DiscoveredService(CBPeripheral peripheral, NSError? error)
        {
            discoveredServiceSubject.OnNext(error);
        }

        public override void DiscoveredCharacteristic(CBPeripheral peripheral, CBService service, NSError? error)
        {
            discoveredCharacteristicSubject.OnNext((service, error));
        }

        public override void DiscoveredDescriptor(CBPeripheral peripheral, CBCharacteristic characteristic, NSError? error)
        {
            discoveredDescriptorSubject.OnNext((characteristic, error));
        }

        public override void UpdatedCharacterteristicValue(CBPeripheral peripheral, CBCharacteristic characteristic, NSError? error)
        {
            updatedCharacteristicValueSubject.OnNext((characteristic, characteristic.Value, error));
        }

        public override void UpdatedValue(CBPeripheral peripheral, CBDescriptor descriptor, NSError? error)
        {
            updatedDescriptorValueSubject.OnNext((descriptor, descriptor.Value, error));
        }

        public override void WroteCharacteristicValue(CBPeripheral peripheral, CBCharacteristic characteristic, NSError? error)
        {
            wroteCharacteristicSubject.OnNext((characteristic, error));
        }

        public override void WroteDescriptorValue(CBPeripheral peripheral, CBDescriptor descriptor, NSError? error)
        {
            wroteDescriptorSubject.OnNext((descriptor, error));
        }

        public override void UpdatedNotificationState(CBPeripheral peripheral, CBCharacteristic characteristic, NSError? error)
        {
            updatedNotificationState.OnNext((characteristic, characteristic.IsNotifying, error));
        }

        public override void RssiRead(CBPeripheral peripheral, NSNumber rssi, NSError? error)
        {
            updatedRssiSubject.OnNext((rssi, error));
        }
    }
}
