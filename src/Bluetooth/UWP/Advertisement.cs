using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Devices.Bluetooth.Advertisement;

namespace Dandy.Devices.Bluetooth
{
    partial class BLEAdvertisement
    {
        private readonly BluetoothLEAdvertisement advertisement;

        internal BLEAdvertisement(BluetoothLEAdvertisement advertisement)
        {
            this.advertisement = advertisement ?? throw new ArgumentNullException(nameof(advertisement));
        }

        string _get_LocalName() => advertisement.LocalName;

        IReadOnlyList<Guid> _get_ServiceUuids() => advertisement.ServiceUuids.ToList().AsReadOnly();
    }
}
