using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Bluetooth;

namespace Dandy.Devices.Bluetooth
{
    partial class Device
    {
        private readonly BluetoothLEDevice device;

        Device(BluetoothLEDevice device)
        {
            this.device = device ?? throw new ArgumentNullException(nameof(device));
        }

        BluetoothAddress _get_BluetoothAddress() => BluetoothAddress.FromULong(device.BluetoothAddress);

        string _get_Name() => device.Name;

        static Task<Device> _FromIdAsync(string id) => BluetoothLEDevice.FromIdAsync(id).AsTask()
            .ContinueWith(a => new Device(a.Result));

        Task<IReadOnlyList<GattService>> _GetGattServicesAsync(Guid uuid)
        {
            return device.GetGattServicesForUuidAsync(uuid).AsTask().ContinueWith<IReadOnlyList<GattService>>(t => {
                switch (t.Result.Status) {
                case Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success:
                    return t.Result.Services.Select(x => new GattService(x)).ToList().AsReadOnly();
                case Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.AccessDenied:
                    throw new AccessViolationException("Access denied.");
                case Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.ProtocolError:
                case Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Unreachable:
                default:
                    throw new Exception("Need a better exception here");
                }
            });
        }

        public void Dispose()
        {
            device.Dispose();
        }
    }
}
