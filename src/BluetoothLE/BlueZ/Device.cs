using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using bluez.DBus;
using Tmds.DBus;

namespace Dandy.Devices.BluetoothLE
{
    partial class Device
    {
        private readonly IDevice1 proxy;
        private IDisposable propertyWatcher;
        readonly private IDictionary<string, object> properties;

        Device(ObjectPath path)
        {
            proxy = Connection.System.CreateProxy<IDevice1>("org.bluez", path);
            properties = new Dictionary<string, object>();
        }

        async Task<Device> InitAsync()
        {
            propertyWatcher = await proxy.WatchPropertiesAsync(x => UpdateProperties(x.Changed));
            UpdateProperties(await proxy.GetAllAsync());
            await proxy.ConnectAsync();

            return this;
        }

        void UpdateProperties(IEnumerable<KeyValuePair<string, object>> changed)
        {
            foreach (var p in changed) {
                properties[p.Key] = p.Value;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            propertyWatcher?.Dispose();
            proxy.DisconnectAsync().ContinueWith(x => {});
        }

        BluetoothAddress _get_BluetoothAddress() => BluetoothAddress.Parse((string)properties["Address"]);

        static Task<Device> _FromIdAsync(string id)
        {
            return new Device(new ObjectPath(id)).InitAsync();
        }

        async Task<IReadOnlyList<GattService>> _GetGattServicesAsync(Guid uuid)
        {
            while (!(bool)properties["ServicesResolved"]) {
                await Task.Delay(10);
                // TODO: need a timeout here!
            }

            var manager = Connection.System.CreateProxy<IObjectManager>("org.bluez", ObjectPath.Root);
            var objs = await manager.GetManagedObjectsAsync();
            var services = new List<GattService>();

            foreach (var service in objs.Where(x => x.Key.StartsWith(proxy.ObjectPath) && x.Value.ContainsKey("org.bluez.GattService1"))) {
                services.Add(await GattService.CreateInstanceAsync(service.Key));
            }

            return services.AsReadOnly();
        }
    }

    static class GattServiceExtensions
    {
        internal static bool StartsWith(this ObjectPath servicePath, ObjectPath devicePath)
        {
            return servicePath.ToString().StartsWith(devicePath.ToString(), StringComparison.Ordinal);
        }
    }
}
