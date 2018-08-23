using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bluez.DBus;
using Tmds.DBus;

namespace Dandy.Devices.Bluetooth
{
    partial class BLEAdvertisementWatcher
    {
        const string gattService = "org.bluez.GattService1";
        const string gattCharacteristic1 = "org.bluez.GattCharacteristic1";

        private IObjectManager proxy;
        private Stack<IDisposable> watchers;

        static void _BLEAdvertisementWatcher(BLEAdvertisementWatcher instance)
        {
            instance.proxy = Connection.System.CreateProxy<IObjectManager>("org.bluez", ObjectPath.Root);
            instance.watchers = new Stack<IDisposable>();
        }

        void _Start()
        {
            Task.Run(async () => {
                watchers.Push(await proxy.WatchInterfacesAddedAsync(OnAdded));
                var devices = await proxy.GetManagedObjectsAsync();
                foreach (var device in devices) {
                    OnAdded((device.Key, device.Value));
                }
            });
        }

        void OnAdded((ObjectPath @object, IDictionary<string, IDictionary<string, object>> interfaces) obj)
        {
            foreach (var iface in obj.interfaces) {
                if (iface.Key != gattService) {
                    continue;
                }
                _Received?.Invoke(this, new BLEAdvertisementReceivedEventArgs(obj.@object, iface.Value));
            }
        }

        void _Stop()
        {
            Task.Run(() => {
                while (watchers.Count > 0) {
                    watchers.Pop().Dispose();
                }
                _Stopped?.Invoke(this, new BLEAdvertisementStoppedEventArgs());
            });
        }

        event EventHandler<BLEAdvertisementReceivedEventArgs> _Received;
        void _add_Received(EventHandler<BLEAdvertisementReceivedEventArgs> value) => _Received += value;
        void _remove_Received(EventHandler<BLEAdvertisementReceivedEventArgs> value) => _Received -= value;


        event EventHandler<BLEAdvertisementStoppedEventArgs> _Stopped;
        void _add_Stopped(EventHandler<BLEAdvertisementStoppedEventArgs> value) => _Stopped += value;
        void _remove_Stopped(EventHandler<BLEAdvertisementStoppedEventArgs> value) => _Stopped -= value;
    }
}
