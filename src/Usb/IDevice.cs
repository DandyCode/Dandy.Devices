using System;
using System.Collections.Generic;
using System.Text;

namespace Dandy.Devices.Usb
{
    public interface IDevice : IDisposable
    {
        IDeviceDescriptor DeviceDescriptor { get; }

    }
}
