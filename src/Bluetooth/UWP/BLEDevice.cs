using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Bluetooth;

namespace Dandy.Devices.Bluetooth
{
    partial class BLEDevice
    {
        private readonly BluetoothLEDevice device;

        internal BLEDevice(BluetoothLEDevice device)
        {
            this.device = device ?? throw new ArgumentNullException(nameof(device));
        }

        BluetoothAddress _get_BluetoothAddress() => BluetoothAddress.FromULong(device.BluetoothAddress);

        string _get_Name() => device.Name;
    }
}
