using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dandy.Devices.BluetoothLE
{
    partial class Advertisement
    {
        string _get_LocalName() => throw new NotImplementedException();
        IReadOnlyList<Guid> _get_ServiceUuids() => throw new NotImplementedException();

        IReadOnlyDictionary<ushort, ReadOnlyMemory<byte>> _get_ManufacturerData() => throw new NotImplementedException();
    }
}
