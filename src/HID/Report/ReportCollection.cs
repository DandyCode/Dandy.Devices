using System.Collections.Generic;

namespace Dandy.Devices.HID.Report
{
    public sealed class ReportCollection
    {
        readonly List<Report> reports = new List<Report>();

        public CollectionType Type { get; }

        public ReportCollection Parent { get; }

        public UsagePage UsagePage { get; }

        public ushort UsageId { get; }

        internal ReportCollection(int data, int usagePage, int usageId, ReportCollection parent)
        {
            Type = (CollectionType)data;
            UsagePage = (UsagePage)usagePage;
            UsageId = (ushort)usageId;
            Parent = parent;
        }

        internal void Add(Report report)
        {
            reports.Add(report);
        }
    }

    public enum CollectionType
    {
        Physical = 0x00, //group of axes
        Application = 0x01, //mouse, keyboard
        Logical = 0x02, //interrelated data
        Report = 0x03,
        NamedArray = 0x04,
        UsageSwitch = 0x05,
        UsageModifier = 0x06,
        Vendor0 = 0x80,
        Vendor127 = 0xFF,
    }
}
