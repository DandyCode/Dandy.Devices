using System;

namespace Dandy.Devices.USB
{
    public sealed class IDeviceDescriptor
    {
        public uint BcdDeviceRevision { get; }
        public uint BcdUsb { get; }
        public byte MaxPacketSize0 { get; }
        public byte NumberOfConfigurations { get; }
        public uint ProductId { get; }
        public uint VendorId { get; }
    }
}
