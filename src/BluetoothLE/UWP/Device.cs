using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Win = Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Dandy.Devices.BluetoothLE
{
    partial class Device
    {
        private readonly BluetoothLEDevice device;

        Device(BluetoothLEDevice device)
        {
            this.device = device ?? throw new ArgumentNullException(nameof(device));
            device.NameChanged += Device_NameChanged;
            device.ConnectionStatusChanged += Device_ConnectionStatusChanged;
        }

        private void Device_NameChanged(BluetoothLEDevice sender, object args)
        {
            OnPropertyChanged(nameof(Name));
        }

        private void Device_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            OnPropertyChanged(nameof(IsConnected));
        }

        BluetoothAddress _get_BluetoothAddress() => BluetoothAddress.FromULong(device.BluetoothAddress);

        string _get_Name() => device.Name;

        bool _get_IsConnected() => device.ConnectionStatus == BluetoothConnectionStatus.Connected;

        static async Task<Device> _FromIdAsync(string id)
        {
            var device = await BluetoothLEDevice.FromIdAsync(id);
            return new Device(device);
        }

        static async Task<Device> _FromAddressAsync(BluetoothAddress address)
        {
            var device = await BluetoothLEDevice.FromBluetoothAddressAsync(address.ToULong());
            return new Device(device);
        }

        async Task<IReadOnlyList<GattService>> _GetGattServicesAsync(Guid uuid)
        {
            var result = await device.GetGattServicesForUuidAsync(uuid);
            switch (result.Status) {
            case Win.GattCommunicationStatus.Success:
                return result.Services.Select(x => new GattService(x)).ToList().AsReadOnly();
            case Win.GattCommunicationStatus.AccessDenied:
                throw new AccessViolationException("Access denied.");
            case Win.GattCommunicationStatus.ProtocolError:
            case Win.GattCommunicationStatus.Unreachable:
            default:
                throw new Exception("Need a better exception here");
            }
        }

        void _Dispose()
        {
            device.NameChanged -= Device_NameChanged;
            device.ConnectionStatusChanged -= Device_ConnectionStatusChanged;
            device.Dispose();
        }
    }
}
