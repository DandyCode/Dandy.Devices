using System;
using System.Collections.Generic;

namespace Dandy.Devices.Bluetooth
{
    partial class BLEAdvertisement
    {
        internal BLEAdvertisement(object x, object y)
        {
        }

        string _get_LocalName() => throw new NotImplementedException();

        IReadOnlyList<Guid> _get_ServiceUuids() => throw new NotImplementedException();
    }
}
