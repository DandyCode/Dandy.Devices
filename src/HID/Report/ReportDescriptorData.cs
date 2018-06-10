using System;
using System.Linq;
using System.Reflection;

namespace Dandy.Devices.HID.Report
{
    /// <summary>
    /// HID report data structure
    /// </summary>
    sealed class ReportDescriptorData
    {
#pragma warning disable CS0649 // values are assigned via reflection
        [Global, Tag(0b0000_01_00)]
        public int? UsagePage;

        [Global, Tag(0b0001_01_00)]
        public int? LogicalMinimum;

        [Global, Tag(0b0010_01_00)]
        public int? LogicalMaximum;

        [Global, Tag(0b0011_01_00)]
        public int? PhysicalMinimum;

        [Global, Tag(0b0100_01_00)]
        public int? PhysicalMaximum;

        [Global, Tag(0b0101_01_00)]
        public int? UnitExponent;

        [Global, Tag(0b0110_01_00)]
        public int? Unit;

        [Global, Tag(0b0111_01_00)]
        public int? ReportSize;

        [Global, Tag(0b1000_01_00)]
        public int? ReportId;

        [Global, Tag(0b1001_01_00)]
        public int? ReportCount;

        [Global, Tag(0b1010_01_00)]
        public int? Push;

        [Global, Tag(0b1011_01_00)]
        public int? Pop;

        [Local, Tag(0b0000_10_00)]
        public int? Usage;

        [Local, Tag(0b0001_10_00)]
        public int? UsageMinimum;

        [Local, Tag(0b0010_10_00)]
        public int? UsageMaximum;

        [Local, Tag(0b0011_10_00)]
        public int? DesignatorIndex;

        [Local, Tag(0b0100_10_00)]
        public int? DesignatorMinimum;

        [Local, Tag(0b0101_10_00)]
        public int? DesignatorMaximum;

        [Local, Tag(0b0111_10_00)]
        public int? StringIndex;

        [Local, Tag(0b1000_10_00)]
        public int? StringMinimum;

        [Local, Tag(0b1001_10_00)]
        public int? StringMaximum;

        [Local, Tag(0b1001_10_00)]
        public int? Delimeter;
#pragma warning restore CS0649

        public ReportDescriptorData Copy()
        {
            return (ReportDescriptorData)MemberwiseClone();
        }

        public void ClearLocals()
        {
            var type = this.GetType();
            var fields = type.GetFields();
            foreach (var f in fields.Where(x => x.GetCustomAttribute<LocalAttribute>() != null)) {
                f.SetValue(this, null);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    sealed class GlobalAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    sealed class LocalAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    sealed class TagAttribute : Attribute
    {
        readonly int prefix;

        public TagAttribute(byte prefix)
        {
            this.prefix = prefix & 0b1111_11_00;
        }

        public bool Match(byte value)
        {
            return (value & 0b1111_11_00) == prefix;
        }
    }
}
