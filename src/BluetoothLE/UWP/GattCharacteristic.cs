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
        private Dictionary<EventHandler<GattValueChangedEventArgs>,
            TypedEventHandler<Win.GattCharacteristic, Win.GattValueChangedEventArgs>> valueChangedHandlerMap;

        static GattCharacteristic()
        {
            // verify assumptions
            if ((int)Win.GattWriteOption.WriteWithResponse != (int)GattWriteOption.WriteWithResponse) {
                throw new TypeLoadException($"Bad {typeof(GattWriteOption)}{nameof(GattWriteOption.WriteWithResponse)} value");
            }
            if ((int)Win.GattWriteOption.WriteWithoutResponse != (int)GattWriteOption.WriteWithoutResponse) {
                throw new TypeLoadException($"Bad {typeof(GattWriteOption)}{nameof(GattWriteOption.WriteWithoutResponse)} value");
            }
        }

        internal GattCharacteristic(Win.GattCharacteristic characteristic)
        {
            this.characteristic = characteristic ?? throw new ArgumentNullException(nameof(characteristic));
            valueChangedHandlerMap = new Dictionary<EventHandler<GattValueChangedEventArgs>,
                TypedEventHandler<Win.GattCharacteristic, Win.GattValueChangedEventArgs>>();
        }

        Guid _get_Uuid() => characteristic.Uuid;

        Task _WriteValueAsync(ReadOnlyMemory<byte> data, GattWriteOption option)
        {
            // REVISIT: copying everything twice
            var writer = new DataWriter();
            writer.WriteBytes(data.ToArray());
            return characteristic.WriteValueWithResultAsync(writer.DetachBuffer(), (Win.GattWriteOption)option).AsTask();
        }

        Task<Memory<byte>> _ReadValueAsync()
        {
            return characteristic.ReadValueAsync().AsTask().ContinueWith(t => {
                switch (t.Result.Status) {
                case Win.GattCommunicationStatus.Success:
                    var data = new byte[t.Result.Value.Length];
                    var reader = DataReader.FromBuffer(t.Result.Value);
                    reader.ReadBytes(data);
                    return (Memory<byte>)data;
                case Win.GattCommunicationStatus.AccessDenied:
                    throw new AccessViolationException("Access denied");
                case Win.GattCommunicationStatus.ProtocolError:
                    throw new FormatException($"Protocol error: {t.Result.ProtocolError}");
                case Win.GattCommunicationStatus.Unreachable:
                    // TODO: more specific exception
                    throw new Exception("Unreachable");
                }
                throw new ArgumentException("Unknown status");
            });
        }

        Task _StartNotifyAsync()
        {
            return characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                Win.GattClientCharacteristicConfigurationDescriptorValue.Notify).AsTask();
        }

        Task _StopNotifyAsync()
        {
            return characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                Win.GattClientCharacteristicConfigurationDescriptorValue.None).AsTask();
        }

        void _add_ValueChanged(EventHandler<GattValueChangedEventArgs> handler)
        {
            TypedEventHandler<Win.GattCharacteristic, Win.GattValueChangedEventArgs> wrapper = (s, e) => {

            };

            valueChangedHandlerMap.Add(handler, wrapper);

            characteristic.ValueChanged += wrapper;
        }

        void _remove_ValueChanged(EventHandler<GattValueChangedEventArgs> handler)
        {
            if (valueChangedHandlerMap.TryGetValue(handler, out var wrapper)) {
                characteristic.ValueChanged -= wrapper;
                valueChangedHandlerMap.Remove(handler);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }
    }
}
