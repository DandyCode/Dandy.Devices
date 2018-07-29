using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dandy.Devices.Serial
{
    public interface IDeviceInfo : IDisposable
    {
        string DisplayName { get;  }
        string PortName { get; }
        ushort UsbVendorId { get; }
        ushort UsbProductId { get; }
        Task<IDevice> OpenAsync();
    }
}
