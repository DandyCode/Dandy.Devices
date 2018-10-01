using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Bluetooth;
using Win = Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Dandy.Devices.BluetoothLE
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

        static Task<Device> _FromAddressAsync(BluetoothAddress address) =>
            BluetoothLEDevice.FromBluetoothAddressAsync(address.ToULong()).AsTask()
            .ContinueWith(t => new Device(t.Result));

        Task<IReadOnlyList<GattService>> _GetGattServicesAsync(Guid uuid)
        {
            return device.GetGattServicesForUuidAsync(uuid).AsTask().ContinueWith<IReadOnlyList<GattService>>(t => {
                switch (t.Result.Status) {
                case Win.GattCommunicationStatus.Success:
                    return t.Result.Services.Select(x => new GattService(x)).ToList().AsReadOnly();
                case Win.GattCommunicationStatus.AccessDenied:
                    throw new AccessViolationException("Access denied.");
                case Win.GattCommunicationStatus.ProtocolError:
                case Win.GattCommunicationStatus.Unreachable:
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
