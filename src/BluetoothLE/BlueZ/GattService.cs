using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bluez.DBus;
using Tmds.DBus;

namespace Dandy.Devices.BluetoothLE
{
    partial class GattService
    {
        private readonly IGattService1 proxy;
        private IDisposable watcher;
        private IDictionary<string, object> properties;

        GattService(ObjectPath path)
        {
            proxy = Connection.System.CreateProxy<IGattService1>("org.bluez", path);
        }

        internal static async Task<GattService> CreateInstanceAsync(ObjectPath path)
        {
            var instance = new GattService(path);
            instance.watcher = await instance.proxy.WatchPropertiesAsync(instance.OnPropertiesChanged);
            instance.properties = await instance.proxy.GetAllAsync();
            return instance;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            watcher?.Dispose();
        }

        private void OnPropertiesChanged(PropertyChanges obj)
        {
            foreach (var prop in obj.Changed) {
                properties[prop.Key] = prop.Value;
            }
        }

        Guid _get_Uuid() => new Guid((string)properties["UUID"]);

        async Task<IReadOnlyList<GattCharacteristic>> _GetCharacteristicsAsync()
        {
            var manager = Connection.System.CreateProxy<IObjectManager>("org.bluez", ObjectPath.Root);
            var objs = await manager.GetManagedObjectsAsync();
            var characteristics = new List<GattCharacteristic>();

            foreach (var characteristic in objs.Where(x => x.Key.StartsWith(proxy.ObjectPath) && x.Value.ContainsKey("org.bluez.GattCharacteristic1"))) {
                characteristics.Add(await GattCharacteristic.CreateInstanceAsync(characteristic.Key));
            }

            return characteristics.AsReadOnly();
        }
    }
}
