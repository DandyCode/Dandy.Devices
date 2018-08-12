using System;
using System.Collections.Generic;
using System.Linq;
using Tmds.DBus;

namespace Dandy.Devices.Bluetooth
{
    partial class DeviceInformationUpdate
    {
        private readonly IDictionary<string, object> properties;

        internal ObjectPath ObjectPath { get; }

        internal string Interface { get; }

        internal DeviceInformationUpdate(ObjectPath @object, string @interface, IDictionary<string, object> properties)
        {
            ObjectPath = @object;
            Interface = @interface ?? throw new ArgumentNullException(nameof(@interface));
            this.properties = properties ?? new Dictionary<string, object>();
        }

        string _get_Id() => ObjectPath.ToString();

        IReadOnlyDictionary<string, object> _get_Properties() => (IReadOnlyDictionary<string, object>)properties;
    }
}
