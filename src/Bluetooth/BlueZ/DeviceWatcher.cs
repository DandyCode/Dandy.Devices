using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bluez.DBus;
using Tmds.DBus;

namespace Dandy.Devices.Bluetooth
{
    partial class DeviceWatcher
    {
        static IReadOnlyList<string> interfaceNames = new List<string> {
            "org.bluez.Adapter1",
            "org.bluez.Device1"
        }.AsReadOnly();

        private readonly IObjectManager proxy;
        private readonly Stack<IDisposable> watchers;

        internal DeviceWatcher()
        {
            proxy = Connection.System.CreateProxy<IObjectManager>("org.bluez", ObjectPath.Root);
            watchers = new Stack<IDisposable>();
        }

        event EventHandler<DeviceInformation> _Added;
        void _add_Added(EventHandler<DeviceInformation> handler) => _Added += handler;
        void _remove_Added(EventHandler<DeviceInformation> handler) => _Added -= handler;

        void _add_Updated(EventHandler<DeviceInformationUpdate> handler) => throw new NotImplementedException();
        void _remove_Updated(EventHandler<DeviceInformationUpdate> handler) => throw new NotImplementedException();

        event EventHandler<DeviceInformationUpdate> _Removed;
        void _add_Removed(EventHandler<DeviceInformationUpdate> handler) => _Removed += handler;
        void _remove_Removed(EventHandler<DeviceInformationUpdate> handler) => _Removed -= handler;

        event EventHandler _EnumerationCompleted;
        void _add_EnumerationCompleted(EventHandler handler) => _EnumerationCompleted += handler;
        void _remove_EnumerationCompleted(EventHandler handler) => _EnumerationCompleted -= handler;

        event EventHandler _Stopped;
        void _add_Stopped(EventHandler handler) => _Stopped += handler;
        void _remove_Stopped(EventHandler handler) => _Stopped -= handler;

        void _Start()
        {
            Task.Run(async () => {
                watchers.Push(await proxy.WatchInterfacesAddedAsync(OnAdded));
                watchers.Push(await proxy.WatchInterfacesRemovedAsync(OnRemoved));
                // TODO: how to watch PropertiesChanged signal for all objects?
                var devices = await proxy.GetManagedObjectsAsync();
                foreach (var device in devices) {
                    OnAdded((device.Key, device.Value));
                }
                _EnumerationCompleted?.Invoke(this, EventArgs.Empty);
            });
        }

        void _Stop()
        {
            Task.Run(() => {
                while (watchers.Count > 0) {
                    watchers.Pop().Dispose();
                }
                _Stopped?.Invoke(this, EventArgs.Empty);
            });
        }

        private void OnAdded((ObjectPath @object, IDictionary<string, IDictionary<string, object>> interfaces) obj)
        {
            foreach (var iface in obj.interfaces) {
                if (!interfaceNames.Contains(iface.Key)) {
                    continue;
                }
                _Added?.Invoke(this, new DeviceInformation(obj.@object, iface.Key, iface.Value));
            }
        }

        private void OnRemoved((ObjectPath @object, string[] interfaces) obj)
        {
            foreach (var iface in obj.interfaces) {
                if (!interfaceNames.Contains(iface)) {
                    continue;
                }
                _Removed?.Invoke(this, new DeviceInformationUpdate(obj.@object, iface, null));
            }
        }
    }
}
