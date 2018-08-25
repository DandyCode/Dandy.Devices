using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tmds.DBus;

namespace Dandy.Devices.Bluetooth
{
    partial class DeviceInfo
    {
        internal ObjectPath ObjectPath { get; }

        internal string Interface { get; }

        private readonly IDictionary<string, object> properties;

        static DeviceWatcher _CreateWatcher()
        {
            return new DeviceWatcher();
        }

        static Task<IEnumerable<DeviceInfo>> _FindAllAsync()
        {
            var completion = new TaskCompletionSource<IEnumerable<DeviceInfo>>();
            var list = new List<DeviceInfo>();

            var watcher = new DeviceWatcher();
            watcher.Added += (s, e) => list.Add(e);
            watcher.EnumerationCompleted += (s, e) => watcher.Stop();
            watcher.Stopped += (s, e) => completion.SetResult(list);
            watcher.Start();

            return completion.Task;
        }

        internal DeviceInfo(ObjectPath @object, string @interface, IDictionary<string, object> properties)
        {
            ObjectPath = @object;
            Interface = @interface ?? throw new ArgumentNullException(nameof(@interface));
            this.properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        string _get_Id() => ObjectPath.ToString();

        string _get_Name() => (string)properties["Alias"];

        BluetoothAddress _get_Address() => BluetoothAddress.Parse((string)properties["Address"]);

        bool _get_IsConnected() => (bool)properties["Connected"];

        IReadOnlyList<Guid> _get_ServiceUuids() => ((string[])properties["UUIDs"]).Select(x => new Guid(x)).ToList().AsReadOnly();

        IReadOnlyDictionary<Guid, ReadOnlyMemory<byte>> _get_ServiceData()
        {
            if (properties.TryGetValue("ServiceData", out var value)) {
                var data = (IDictionary<string, object>)value;
                return data?.ToDictionary(x => new Guid(x.Key), x => new ReadOnlyMemory<byte>((byte[])x.Value));
            }

            return null;
        }

        IReadOnlyDictionary<ushort, ReadOnlyMemory<byte>> _get_ManufacturerData()
        {
            var data = (IDictionary<ushort, object>)properties["ManufacturerData"];
            return data.ToDictionary(x => x.Key, x => new ReadOnlyMemory<byte>((byte[])x.Value));
        }

        short _get_TxPower() => (short)properties["TxPower"];

        IReadOnlyDictionary<string, object> _get_Properties() => (IReadOnlyDictionary<string, object>)properties;

        void _Update(DeviceInfoUpdate updateInfo)
        {
            if (updateInfo == null) {
                throw new ArgumentNullException(nameof(updateInfo));
            }
            if (updateInfo.ObjectPath != ObjectPath || updateInfo.Interface != Interface) {
                throw new ArgumentException("Device path or interface mismatch", nameof(updateInfo));
            }

            foreach (var p in updateInfo.Properties) {
                properties[p.Key] = p.Value;
            }
        }
    }
}
