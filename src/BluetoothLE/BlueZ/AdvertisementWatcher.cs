using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bluez.DBus;
using Tmds.DBus;

namespace Dandy.Devices.BluetoothLE
{
    enum AdvertisementWatcherStatus
    {
         Created,
         Started,
         Stopping,
         Stopped,
         Aborted,
    }

    partial class AdvertisementWatcher
    {
        private readonly IObjectManager proxy;
        private readonly Dictionary<ObjectPath, Dictionary<string, object>> deviceProperties;
        private readonly Stack<Func<Task>> stopActions;
        private readonly Guid[] uuids;
        private AdvertisementWatcherStatus status;
        private AdvertisementWatcherError stopError;

        internal AdvertisementWatcher(object obj)
        {
            proxy = Connection.System.CreateProxy<IObjectManager>("org.bluez", ObjectPath.Root);
            deviceProperties = new Dictionary<ObjectPath, Dictionary<string, object>>();
            stopActions = new Stack<Func<Task>>();
            this.uuids = (Guid[])obj;;
        }

        void _Start()
        {
            if (status == AdvertisementWatcherStatus.Started) {
                throw new InvalidOperationException("Watcher is already running");
            }
            if (status == AdvertisementWatcherStatus.Stopping) {
                throw new InvalidOperationException("Watcher is still stopping");
            }

            // following Windows API by setting Started status immediately
            status = AdvertisementWatcherStatus.Started;
            stopError = AdvertisementWatcherError.None;

            StartAsync().ContinueWith(t => {
                if (t.Status != TaskStatus.RanToCompletion) {
                    status = AdvertisementWatcherStatus.Aborted;
                    // TODO: more specific errors based on exception
                    stopError = AdvertisementWatcherError.Unknown;
                    Stop();
                }
            });
        }

        async Task StartAsync()
        {
            var addedWatcher = await proxy.WatchInterfacesAddedAsync(OnAdded);
            stopActions.Push(() => Task.Run(() => addedWatcher.Dispose()));

            var devices = await proxy.GetManagedObjectsAsync();
            foreach (var device in devices) {
                foreach (var iface in device.Value) {
                    if (iface.Key == "org.bluez.Adapter1") {
                        var adapter = Connection.System.CreateProxy<IAdapter1>("org.bluez", device.Key);
                        try {
                            await adapter.SetDiscoveryFilterAsync(new Dictionary<string, object> {
                                { "Transport", "le" },
                                { "UUIDs", uuids.Select(x => x.ToString()).ToArray() },
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
            if (status != AdvertisementWatcherStatus.Started) {
                throw new InvalidOperationException("Watcher is not running");
            }

            status = AdvertisementWatcherStatus.Stopping;

            StopAsync().ContinueWith(t => {
                if (t.IsCompleted) {
                    status = AdvertisementWatcherStatus.Stopped;
                    Stopped?.Invoke(this, new AdvertisementWatcherStoppedEventArgs(stopError));
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
                if (!deviceProperties.TryGetValue(obj.@object, out var properties)) {
                    properties = new Dictionary<string, object>();
                    deviceProperties.Add(obj.@object, properties);
                }
                foreach (var kvp in iface.Value) {
                    properties[kvp.Key] = kvp.Value;
                }

                // We only publish property changes as advertisements to avoid
                // cached devices
                deviceProxy.WatchPropertiesAsync(changes => {
                    foreach (var kvp in changes.Changed) {
                        properties[kvp.Key] = kvp.Value;
                    }

                    var advertisement = new Advertisement(properties);
                    var address = BluetoothAddress.Parse((string)properties["Address"]);
                    var rssi =(short)properties["RSSI"];

                    Received?.Invoke(this, new AdvertisementReceivedEventArgs(advertisement, address, rssi));
                }).ContinueWith(t => {
                    stopActions.Push(() => Task.Run(() => t.Result.Dispose()));
                });
            }
        }
    }
}
