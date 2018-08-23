using Dandy.Libusb;

namespace Dandy.Devices.Usb
{
    partial class Interface
    {
        private readonly Libusb.Interface @interface;

        internal Interface(Libusb.Interface @interface)
        {
            this.@interface = @interface;

        }
    }
}
