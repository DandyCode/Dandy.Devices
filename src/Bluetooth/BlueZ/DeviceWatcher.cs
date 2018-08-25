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
        private readonly IObjectManager proxy;
        private readonly Stack<Func<Task>> stopActions;

        internal DeviceWatcher()
        {
            proxy = Connection.System.CreateProxy<IObjectManager>("org.bluez", ObjectPath.Root);
            stopActions = new Stack<Func<Task>>();
        }

        event EventHandler<DeviceInfo> _Added;
        void _add_Added(EventHandler<DeviceInfo> handler) => _Added += handler;
        void _remove_Added(EventHandler<DeviceInfo> handler) => _Added -= handler;

        event EventHandler<DeviceInfoUpdate> _Updated;
        void _add_Updated(EventHandler<DeviceInfoUpdate> handler) => _Updated += handler;
        void _remove_Updated(EventHandler<DeviceInfoUpdate> handler) => _Updated -= handler;

        event EventHandler<DeviceInfoUpdate> _Removed;
        void _add_Removed(EventHandler<DeviceInfoUpdate> handler) => _Removed += handler;
        void _remove_Removed(EventHandler<DeviceInfoUpdate> handler) => _Removed -= handler;

        event EventHandler _EnumerationCompleted;
        void _add_EnumerationCompleted(EventHandler handler) => _EnumerationCompleted += handler;
        void _remove_EnumerationCompleted(EventHandler handler) => _EnumerationCompleted -= handler;

        event EventHandler _Stopped;
        void _add_Stopped(EventHandler handler) => _Stopped += handler;
        void _remove_Stopped(EventHandler handler) => _Stopped -= handler;

        void _Start()
        {
            if (stopActions.Count > 0) {
                throw new InvalidOperationException("Watcher is already running");
            }

            StartAsync().ContinueWith(t => {
                if (t.IsCompleted) {
                    _EnumerationCompleted?.Invoke(this, EventArgs.Empty);
                }
            });
        }

        async Task StartAsync()
        {
            var addedWatcher = await proxy.WatchInterfacesAddedAsync(OnAdded);
            stopActions.Push(() => Task.Run(() => addedWatcher.Dispose()));
            var removedWatcher = await proxy.WatchInterfacesRemovedAsync(OnRemoved);
            stopActions.Push(() => Task.Run(() => removedWatcher.Dispose()));

            var devices = await proxy.GetManagedObjectsAsync();
            foreach (var device in devices) {
                foreach (var iface in device.Value) {
                    if (iface.Key == "org.bluez.Adapter1") {
                        var adapter = Connection.System.CreateProxy<IAdapter1>("org.bluez", device.Key);
                        try {
                            await adapter.SetDiscoveryFilterAsync(new Dictionary<string, object> {
                                { "Transport", "le" }
                            });
                            stopActions.Push(() => adapter.SetDiscoveryFilterAsync(new Dictionary<string, object>()));

                            await adapter.StartDiscoveryAsync();
                            stopActions.Push(() => adapter.StopDiscoveryAsync());
                        }
                        catch {
                            // we just do the best we can
                        }
                    }
                    else if (iface.Key == "org.bluez.Device1") {
                        OnAdded((device.Key, device.Value));
                    }
                }
            }
        }

        void _Stop()
        {
            if (stopActions.Count == 0) {
                throw new InvalidOperationException("Watcher is not running");
            }

            StopAsync().ContinueWith(t => {
                if (t.IsCompleted) {
                    _Stopped?.Invoke(this, EventArgs.Empty);
                }
            });
        }

        async Task StopAsync()
        {
            while (stopActions.Count > 0) {
                try {
                    var task = stopActions.Pop();
                    await task();
                }
                catch {
                    // we tried
                }
            }
        }

        private void OnAdded((ObjectPath @object, IDictionary<string, IDictionary<string, object>> interfaces) obj)
        {
            foreach (var iface in obj.interfaces) {
                if (iface.Key != "org.bluez.Device1") {
                    continue;
                }

                var deviceProxy = Connection.System.CreateProxy<IDevice1>("org.bluez", obj.@object);

                // TODO: we can't use the properties passed in because of race
                // condition with subscribing to property changes. A possible
                // solution would be to modify the D-Bus library to allow watching
                // properties based on at 'path_namespace=...' match rule.
                // See: https://github.com/tmds/Tmds.DBus/issues/57
                deviceProxy.WatchPropertiesAsync(changes => {
                    _Updated?.Invoke(this, new DeviceInfoUpdate(obj.@object, iface.Key,
                        changes.Changed.ToDictionary(x => x.Key, x => x.Value)));
                }).ContinueWith(t => {
                    stopActions.Push(() => Task.Run(() => t.Result.Dispose()));
                    deviceProxy.GetAllAsync().ContinueWith(t2 => {
                        _Added?.Invoke(this, new DeviceInfo(obj.@object, iface.Key, t2.Result));
                    });
                });
            }
        }

        private void OnRemoved((ObjectPath @object, string[] interfaces) obj)
        {
            foreach (var iface in obj.interfaces) {
                if (iface != "org.bluez.Device1") {
                    continue;
                }
                _Removed?.Invoke(this, new DeviceInfoUpdate(obj.@object, iface, null));
            }
        }
    }
}
