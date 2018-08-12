using System;
using System.Linq;
using System.Collections.Generic;

namespace Dandy.Devices.Bluetooth
{partial class Advertisement
    {
        private readonly Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisement advertisement;

        internal Advertisement(Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisement advertisement)
        {
            this.advertisement = advertisement ?? throw new ArgumentNullException(nameof(advertisement));
        }

        string _get_LocalName() => advertisement.LocalName;

        IReadOnlyList<Guid> _get_ServiceUuids() => advertisement.ServiceUuids.ToList().AsReadOnly();
    }
}
