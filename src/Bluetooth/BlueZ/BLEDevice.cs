using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using bluez.DBus;
using Tmds.DBus;

namespace Dandy.Devices.Bluetooth
{
    partial class BLEDevice
    {
        private readonly IDevice1 proxy;
        private readonly IDictionary<string, object> properties;
        private IDisposable propertyWatcher;

        BLEDevice(ObjectPath path)
        {
            proxy = Connection.System.CreateProxy<IDevice1>("org.bluez.Device1", path);
            properties = new Dictionary<string, object>();
        }

        async Task<BLEDevice> InitAsync()
        {
            var initialProperties = await proxy.GetAllAsync();
            foreach (var p in initialProperties) {
                properties[p.Key] = p.Value;
            }

            return this;
        }

        BluetoothAddress _get_BluetoothAddress() => BluetoothAddress.Parse((string)properties["Address"]);

        string _get_Name() => (string)properties["Alias"];

        static Task<BLEDevice> _FromIdAsync(string id)
        {
            return new BLEDevice(new ObjectPath(id)).InitAsync();
        }
    }
}
