using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Bluetooth;

namespace Dandy.Devices.Bluetooth
{
    partial class BLEDevice
    {
        private readonly BluetoothLEDevice device;

        BLEDevice(BluetoothLEDevice device)
        {
            this.device = device ?? throw new ArgumentNullException(nameof(device));
        }

        BluetoothAddress _get_BluetoothAddress() => BluetoothAddress.FromULong(device.BluetoothAddress);

        string _get_Name() => device.Name;

        static Task<BLEDevice> _FromIdAsync(string id) => BluetoothLEDevice.FromIdAsync(id).AsTask().ContinueWith(a => new BLEDevice(a.Result));
    }
}
