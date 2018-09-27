using System;
using System.Collections.Generic;
using System.Linq;
using Tmds.DBus;

namespace Dandy.Devices.BluetoothLE
{
    partial class DeviceInfoUpdate
    {
        private readonly IDictionary<string, object> properties;

        internal ObjectPath ObjectPath { get; }

        internal string Interface { get; }

        internal DeviceInfoUpdate(ObjectPath @object, string @interface, IDictionary<string, object> properties)
        {
            ObjectPath = @object;
            Interface = @interface ?? throw new ArgumentNullException(nameof(@interface));
            this.properties = properties ?? new Dictionary<string, object>();
        }

        string _get_Id() => ObjectPath.ToString();

        IReadOnlyDictionary<string, object> _get_Properties() => (IReadOnlyDictionary<string, object>)properties;
    }
}
