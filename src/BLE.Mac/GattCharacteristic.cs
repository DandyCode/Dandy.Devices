#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CoreBluetooth;
using Foundation;

namespace Dandy.Devices.BLE.Mac
{
    public class GattCharacteristic
    {
        private readonly CBPeripheral peripheral;
        private readonly PeripheralDelegate @delegate;
        private readonly CBCharacteristic characteristic;

        internal GattCharacteristic(
            CBPeripheral peripheral,
            PeripheralDelegate @delegate,
            GattService service,
            CBCharacteristic characteristic)
        {
            this.peripheral = peripheral;
            this.@delegate = @delegate;
            Service = service;
            this.characteristic = characteristic;
        }

        public GattService Service { get; }

        public Guid Uuid => Marshal.CBUuidToGuid(characteristic.UUID);

        public GattCharacteristicProperties Properties =>
            (GattCharacteristicProperties)characteristic.Properties;

        public async Task<IEnumerable<GattDescriptor>> GetDescriptorsAsync()
        {
            var errorAwaiter = @delegate.DiscoveredDescriptorObservable
                .FirstAsync(x => x.characteristic == characteristic).GetAwaiter();

            peripheral.DiscoverDescriptors(characteristic);
            var (_, error) = await errorAwaiter;

            if (error is not null) {
                throw new NSErrorException(error);
            }

            return characteristic.Descriptors.Select(
                descriptor => new GattDescriptor(peripheral, @delegate, this, descriptor));
        }

        public async Task<ReadOnlyMemory<byte>> ReadAsync()
        {
            var errorAwaiter = @delegate.UpdatedCharacteristicValueObservable
                .FirstAsync(x => x.characteristic == characteristic).GetAwaiter();

            peripheral.ReadValue(characteristic);
            var (_, value, error) = await errorAwaiter;

            if (error is not null) {
                throw new NSErrorException(error);
            }

            return value.ToArray();
        }

        public async Task WriteAsync(ReadOnlyMemory<byte> value, bool withResponse = true)
        {
            var data = Marshal.MemoryToNSData(value);

            if (withResponse) {
                var errorAwaiter = @delegate.WroteCharacteristicObservable
                    .FirstAsync(x => x.characteristic == characteristic).GetAwaiter();

                peripheral.WriteValue(data, characteristic, CBCharacteristicWriteType.WithResponse);
                var (_, error) = await errorAwaiter;

                if (error is not null) {
                    throw new NSErrorException(error);
                }
            }
            else {
                peripheral.WriteValue(data, characteristic, CBCharacteristicWriteType.WithoutResponse);
            }
        }
    }

    [Flags]
    public enum GattCharacteristicProperties : ushort
    {
        /// <summary>
        /// The characteristic doesn’t have any properties that apply.
        /// </summary>
        None = 0,

        /// <summary>
        /// The characteristic supports broadcasting
        /// </summary>
        Broadcast = 1 << 0,

        /// <summary>
        /// The characteristic is readable
        /// </summary>
        Read = 1 << 1,

        /// <summary>
        /// The characteristic supports Write Without Response
        /// </summary>
        WriteWithoutResponse = 1 << 2,

        /// <summary>
        /// The characteristic is writable
        /// </summary>
        Write = 1 << 3,

        /// <summary>
        /// The characteristic is notifiable
        /// </summary>
        Notify = 1 << 4,

        /// <summary>
        /// The characteristic is indicatable
        /// </summary>
        Indicate = 1 << 5,

        /// <summary>
        /// The characteristic supports signed writes
        /// </summary>
        AuthenticatedSignedWrites = 1 << 6,

        /// <summary>
        /// The ExtendedProperties Descriptor is present
        /// </summary>
        ExtendedProperties = 1 << 7,

        /// <summary>
        /// The characteristic supports reliable writes
        /// </summary>
        ReliableWrites = 1 << 8,

        /// <summary>
        /// The characteristic has writable auxiliaries
        /// </summary>
        WritableAuxiliaries = 1 << 9,
    }
}
