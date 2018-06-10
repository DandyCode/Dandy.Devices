using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Dandy.Devices.HID;
using Dandy.Devices.HID.Report;

namespace Dandy.Devices.HID.Linux
{
    sealed class Device : IDevice
    {
        private readonly ReportCollection application;
        private readonly Hidraw hidraw;

        public Device(ReportCollection application, Hidraw hidraw)
        {
            this.application = application ?? throw new ArgumentNullException(nameof(application));
            this.hidraw = hidraw ?? throw new ArgumentNullException(nameof(hidraw));
            
        }

        public string Id => hidraw.PhysicalLocation;

        public string DisplayName => hidraw.Name;

        public ushort UsagePage => (ushort)application.UsagePage;

        public ushort UsageId => application.UsageId;

        public ushort VendorId => hidraw.VendorId;

        public ushort ProductId => hidraw.ProductId;
    }
}
