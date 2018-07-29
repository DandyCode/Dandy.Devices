using System;

namespace Dandy.Devices.Usb
{
    public interface IDeviceDescriptor
    {
        ushort BcdDeviceRevision { get; }
        ushort BcdUsb { get; }
        byte MaxPacketSize0 { get; }
        byte NumberOfConfigurations { get; }
        ushort ProductId { get; }
        ushort VendorId { get; }
    }
}
