using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Dandy.Devices.BluetoothLE
{
    partial class Device
    {
        string _get_Name() => throw new NotImplementedException();

        bool _get_IsConnected() => throw new NotImplementedException();

        BluetoothAddress _get_BluetoothAddress() => throw new NotImplementedException();

        static Task<Device> _FromIdAsync(string id) => throw new NotImplementedException();

        static Task<Device> _FromAddressAsync(BluetoothAddress address) => throw new NotImplementedException();

        Task<IReadOnlyList<GattService>> _GetGattServicesAsync(Guid uuid) => throw new NotImplementedException();

        void _Dispose() => throw new NotImplementedException();
    }
}
