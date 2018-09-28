using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;

namespace Dandy.Devices.BluetoothLE
{
    partial class Advertisement
    {
        private readonly BluetoothLEAdvertisement ad;

        internal Advertisement(BluetoothLEAdvertisement ad)
        {
            this.ad = ad ?? throw new ArgumentNullException(nameof(ad));
        }
        
        string _get_LocalName() => ad.LocalName;
        
        IReadOnlyList<Guid> _get_ServiceUuids() => ad.ServiceUuids.ToList().AsReadOnly();
        
        IReadOnlyDictionary<ushort, ReadOnlyMemory<byte>> _get_ManufacturerData() => ad.ManufacturerData
            .ToDictionary(x => x.CompanyId, x => x.Data.ToReadOnlyMemory());
    }
}
