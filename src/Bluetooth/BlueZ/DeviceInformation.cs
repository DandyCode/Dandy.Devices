using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tmds.DBus;

namespace Dandy.Devices.Bluetooth
{
    partial class DeviceInformation
    {
        internal ObjectPath ObjectPath { get; }

        internal string Interface { get; }

        private readonly IDictionary<string, object> properties;

        static DeviceWatcher _CreateWatcher()
        {
            return new DeviceWatcher();
        }

        static Task<IEnumerable<DeviceInformation>> _FindAllAsync()
        {
            var completion = new TaskCompletionSource<IEnumerable<DeviceInformation>>();
            var list = new List<DeviceInformation>();

            var watcher = new DeviceWatcher();
            watcher.Added += (s, e) => list.Add(e);
            watcher.EnumerationCompleted += (s, e) => watcher.Stop();
            watcher.Stopped += (s, e) => completion.SetResult(list);
            watcher.Start();

            return completion.Task;
        }

        internal DeviceInformation(ObjectPath @object, string @interface, IDictionary<string, object> properties)
        {
            ObjectPath = @object;
            Interface = @interface ?? throw new ArgumentNullException(nameof(@interface));
            this.properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        string _get_Id() => ObjectPath.ToString();

        string _get_Name() => (string)properties["Alias"];

        IReadOnlyDictionary<string, object> _get_Properties() => (IReadOnlyDictionary<string, object>)properties;

        void _Update(DeviceInformationUpdate updateInfo)
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
