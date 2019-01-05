using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;
using Win = Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Dandy.Devices.BluetoothLE
{
    partial class GattCharacteristic
    {
        private readonly Win.GattCharacteristic characteristic;

        internal GattCharacteristic(Win.GattCharacteristic characteristic)
        {
            this.characteristic = characteristic ?? throw new ArgumentNullException(nameof(characteristic));
            characteristic.ValueChanged += Characteristic_ValueChanged;
        }

        Guid _get_Uuid() => characteristic.Uuid;

        async Task _WriteValueAsync(ReadOnlyMemory<byte> data, GattWriteOption option)
        {
            // REVISIT: copying everything twice
            var writer = new DataWriter();
            writer.WriteBytes(data.ToArray());
            switch (option) {
            case GattWriteOption.WriteWithResponse:
                var result = await characteristic.WriteValueWithResultAsync(writer.DetachBuffer());
                    switch (result.Status) {
                case Win.GattCommunicationStatus.Success:
                    return;
                default:
                    throw new Exception();
                }
            case GattWriteOption.WriteWithoutResponse:
                var result2 = await characteristic.WriteValueAsync(writer.DetachBuffer(), Win.GattWriteOption.WriteWithoutResponse);
                switch (result2) {
                case Win.GattCommunicationStatus.Success:
                    return;
                default:
                    throw new Exception();
                }
            default:
                throw new ArgumentException("Invalid write option", nameof(option));
            }
        }

        async Task<Memory<byte>> _ReadValueAsync()
        {
            var result = await characteristic.ReadValueAsync();
            switch (result.Status) {
            case Win.GattCommunicationStatus.Success:
                var data = new byte[result.Value.Length];
                var reader = DataReader.FromBuffer(result.Value);
                reader.ReadBytes(data);
                return (Memory<byte>)data;
            case Win.GattCommunicationStatus.AccessDenied:
                throw new AccessViolationException("Access denied");
            case Win.GattCommunicationStatus.ProtocolError:
                throw new FormatException($"Protocol error: {result.ProtocolError}");
            case Win.GattCommunicationStatus.Unreachable:
                // TODO: more specific exception
                throw new Exception("Unreachable");
            }
            throw new ArgumentException("Unknown status");
        }

        async Task _StartNotifyAsync()
        {
            var result = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                Win.GattClientCharacteristicConfigurationDescriptorValue.Notify);
            switch (result) {
            case Win.GattCommunicationStatus.Success:
                break;
            case Win.GattCommunicationStatus.AccessDenied:
                throw new AccessViolationException();
            case Win.GattCommunicationStatus.ProtocolError:
            case Win.GattCommunicationStatus.Unreachable:
                throw new Exception();
            default:
                throw new InvalidOperationException();
            }
        }

        async Task _StopNotifyAsync()
        {
            var result = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                Win.GattClientCharacteristicConfigurationDescriptorValue.None);
            switch (result) {
            case Win.GattCommunicationStatus.Success:
                break;
            case Win.GattCommunicationStatus.AccessDenied:
                throw new AccessViolationException();
            case Win.GattCommunicationStatus.ProtocolError:
            case Win.GattCommunicationStatus.Unreachable:
                throw new Exception();
            default:
                throw new InvalidOperationException();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        private void Characteristic_ValueChanged(Win.GattCharacteristic sender, Win.GattValueChangedEventArgs args)
        {
            OnValueChanged(args.CharacteristicValue.ToMemory());
        }
    }
}
