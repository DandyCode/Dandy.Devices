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
        private readonly Device device;
        private readonly IGattService1 proxy;
        private IDisposable watcher;
        private IDictionary<string, object> properties;

        GattService(Device device, ObjectPath path)
        {
            this.device = device ?? throw new ArgumentNullException(nameof(device));
            proxy = Connection.System.CreateProxy<IGattService1>("org.bluez", path);
        }

        internal static async Task<GattService> CreateInstanceAsync(Device device, ObjectPath path)
        {
            var instance = new GattService(device, path);
            instance.watcher = await instance.proxy.WatchPropertiesAsync(instance.OnPropertiesChanged);
            instance.properties = await instance.proxy.GetAllAsync();
            return instance;
        }

        void _Dispose() => watcher?.Dispose();

        private void OnPropertiesChanged(PropertyChanges obj)
        {
            foreach (var prop in obj.Changed) {
                properties[prop.Key] = prop.Value;
            }
        }

        Guid _get_Uuid() => new Guid((string)properties["UUID"]);

        Device _get_Device() => device;

        async Task<IReadOnlyList<GattCharacteristic>> _GetCharacteristicsAsync(Guid uuid)
        {
            var manager = Connection.System.CreateProxy<IObjectManager>("org.bluez", ObjectPath.Root);
            var objs = await manager.GetManagedObjectsAsync();
            var characteristics = new List<GattCharacteristic>();

            foreach (var characteristic in objs.Where(x => x.Key.StartsWith(proxy.ObjectPath)
                     && x.Value.TryGetValue("org.bluez.GattCharacteristic1", out var characteristic)
                     && Guid.Parse((string)characteristic["UUID"]) == uuid)) {
                characteristics.Add(await GattCharacteristic.CreateInstanceAsync(this, characteristic.Key));
            }

            return characteristics.AsReadOnly();
        }
    }
}
