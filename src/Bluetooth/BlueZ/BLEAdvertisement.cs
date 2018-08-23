using System;
using System.Collections.Generic;

namespace Dandy.Devices.Bluetooth
{
    partial class BLEAdvertisement
    {
        internal BLEAdvertisement()
        {

        }

        public string LocalName => _get_LocalName();

        public IReadOnlyList<Guid> ServiceUuids => _get_ServiceUuids();
    }
}
