using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dandy.Devices.BluetoothLE
{
    partial class Advertisement
    {
        private readonly Dictionary<string, object> properties;


        internal Advertisement(Dictionary<string, object> properties)
        {
            this.properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        string _get_LocalName() => (string)properties["Alias"];

        IReadOnlyList<Guid> _get_ServiceUuids() =>
            ((string[])properties["UUIDs"]).Select(x => Guid.Parse(x)).ToList().AsReadOnly();

        IReadOnlyDictionary<ushort, ReadOnlyMemory<byte>> _get_ManufacturerData()
        {
            var dict = new Dictionary<ushort, ReadOnlyMemory<byte>>();
            foreach (var kvp in (IDictionary<ushort, byte[]>)properties["ManufacturerData"]) {
                dict[kvp.Key] = kvp.Value;
            }
            return dict;
        }
    }
}
