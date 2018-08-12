using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using bluez.DBus;
using Tmds.DBus;

namespace Dandy.Devices.Bluetooth
{
    partial class Adapter
    {
        private readonly IAdapter1 proxy;
        private readonly IDictionary<string, object> properties;
        private IDisposable propertyWatcher;

        Adapter(ObjectPath path)
        {
            proxy = Connection.System.CreateProxy<IAdapter1>("org.bluez.Adapter1", path);
            properties = new Dictionary<string, object>();
        }

        async Task<Adapter> InitAsync()
        {
            var initialProperties = await proxy.GetAllAsync();
            foreach (var p in initialProperties) {
                properties[p.Key] = p.Value;
            }

            return this;
        }

        BluetoothAddress _get_BluetoothAddress() => BluetoothAddress.Parse((string)properties["Address"]);

        static Task<Adapter> _FromIdAsync(string id)
        {
            return new Adapter(new ObjectPath(id)).InitAsync();
        }
    }
}
