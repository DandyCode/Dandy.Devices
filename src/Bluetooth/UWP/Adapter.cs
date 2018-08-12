using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Bluetooth;

namespace Dandy.Devices.Bluetooth
{
    partial class Adapter
    {
        private readonly BluetoothAdapter adapter;

        Adapter(BluetoothAdapter adapter)
        {
            this.adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }

        BluetoothAddress _get_BluetoothAddress() => BluetoothAddress.FromUint(adapter.BluetoothAddress);

        static Task<Adapter> _FromIdAsync(string id) => BluetoothAdapter.FromIdAsync(id).AsTask().ContinueWith(a => new Adapter(a.Result));
    }
}
