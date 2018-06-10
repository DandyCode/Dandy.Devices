using System;

namespace Dandy.Devices.HID.Report
{
    /// <summary>
    /// HID report data structure
    /// </summary>
    public abstract class Report
    {
        readonly ReportDescriptorData descriptor;

        /// <summary>
        /// Unsigned integer specifying the current Usage Page.
        /// </summary>
        /// <remarks>
        /// Since a usage are 32 bit values, Usage
        /// Page items can be used to conserve space in a
        /// report descriptor by setting the high order 16 bits
        /// of a subsequent usages. Any usage that follows
        /// which is defines 16 bits or less is interpreted as a
        /// Usage ID and concatenated with the Usage Page
        /// to form a 32 bit Usage.
        /// </remarks>
        public UsagePage? UsagePage => (UsagePage?)descriptor.UsagePage;

        /// <summary>
        /// Extent value in logical units.
        /// </summary>
        /// <remarks>
        /// This is the minimum value that a variable or array item will
        /// report. For example, a mouse reporting x
        /// position values from 0 to 128 would have a
        /// Logical Minimum of 0 and a Logical Maximum
        /// of 128.
        /// </remarks>
        public int? LogicalMinimum => descriptor.LogicalMinimum;

        /// <summary>
        /// Extent value in logical units.
        /// </summary>
        /// <remarks>
        /// This is the maximum value that a variable or array item will report.
        /// </remarks>
        public int? LogicalMaximum => descriptor.LogicalMaximum;

        /// <summary>
        /// Minimum value for the physical extent of a variable item.
        /// </summary>
        /// <remarks>
        /// This represents the Logical Minimum with units applied to it.
        /// </remarks>
        public int? PhysicalMinimum => descriptor.PhysicalMinimum;

        /// <summary>
        /// Maximum value for the physical extent of a variable item.
        /// </summary>
        public int? PhysicalMaximum => descriptor.PhysicalMaximum;

        /// <summary>
        /// Value of the unit exponent in base 10. See the
        /// table later in this section for more information.
        /// </summary>
        public UnitExponent? UnitExponent => (UnitExponent?)descriptor.UnitExponent;

        /// <summary>
        /// Unit values.
        /// </summary>
        public Unit? Unit => (Unit?)descriptor.Unit;

        /// <summary>
        /// Unsigned integer specifying the size of the report fields in bits.
        /// </summary>
        /// <remarks>
        /// This allows the parser to build an
        /// item map for the report handler to use. For more
        /// information, see Section 8: Report Protocol.
        /// </remarks>
        public uint? ReportSize => (uint?)descriptor.ReportSize;

        /// <summary>
        /// Unsigned value that specifies the Report ID.
        /// </summary>
        /// <remarks>
        /// Report ID tag is used anywhere in Report
        /// descriptor, all data reports for the device are
        /// preceded by a single byte ID field. All items
        /// succeeding the first Report ID tag but preceding
        /// a second Report ID tag are included in a report
        /// prefixed by a 1-byte ID. All items succeeding the
        /// second but preceding a third Report ID tag are
        /// included in a second report prefixed by a second
        /// ID, and so on.
        ///
        /// This Report ID value indicates the prefix added
        /// to a particular report. For example, a Report
        /// descriptor could define a 3-byte report with a
        /// Report ID of 01. This device would generate a
        /// 4-byte data report in which the first byte is 01.
        /// The device may also generate other reports, each
        /// with a unique ID. This allows the host to
        /// distinguish different types of reports arriving
        /// over a single interrupt in pipe. And allows the
        /// device to distinguish different types of reports
        /// arriving over a single interrupt out pipe. Report
        /// ID zero is reserved and should not be used.
        /// </remarks>
        public uint? ReportId => (uint?)descriptor.ReportId;

        /// <summary>
        /// Unsigned integer specifying the number of data fields for the item.
        /// </summary>
        /// <remarks>
        /// Determines how many fields
        /// are included in the report for this particular item
        /// (and consequently how many bits are added to the report).
        /// </remarks>
        public uint? ReportCount => (uint?)descriptor.ReportCount;

        /// <summary>
        /// Usage index for an item usage.
        /// </summary>
        /// <remarks>
        /// Represents a suggested usage for the item or collection. In the
        /// case where an item represents multiple controls, a
        /// Usage tag may suggest a usage for every variable
        /// or element in an array.
        /// </remarks>
        public uint? Usage => (uint?)descriptor.Usage;

        /// <summary>
        /// Defines the starting usage associated with an array or bitmap.
        /// </summary>
        public uint? UsageMinimum => (uint?)descriptor.UsageMinimum;

        /// <summary>
        /// Defines the ending usage associated with an array or bitmap.
        /// </summary>
        public uint? UsageMaximum => (uint?)descriptor.UsageMaximum;

        /// <summary>
        /// Determines the body part used for a control. Index
        /// points to a designator in the Physical descriptor.
        /// </summary>
        public uint? DesignatorIndex => (uint?)descriptor.DesignatorIndex;

        /// <summary>
        /// Defines the index of the starting designator
        /// associated with an array or bitmap.
        /// </summary>
        public uint? DesignatorMinimum => (uint?)descriptor.DesignatorMinimum;

        /// <summary>
        /// Defines the index of the ending designator
        /// associated with an array or bitmap.
        /// </summary>
        public uint? DesignatorMaximum => (uint?)descriptor.DesignatorMaximum;

        /// <summary>
        /// String index for a String descriptor; allows a string
        /// to be associated with a particular item or control.
        /// </summary>
        public uint? StringIndex => (uint?)descriptor.StringIndex;

        /// <summary>
        /// Specifies the first string index when assigning a
        /// group of sequential strings to controls in an array
        /// or bitmap.
        /// </summary>
        public uint? StringMinimum => (uint?)descriptor.StringMinimum;

        /// <summary>
        /// Specifies the last string index when assigning a
        /// group of sequential strings to controls in an array
        /// or bitmap.
        /// </summary>
        public uint? StringMaximum => (uint?)descriptor.StringMaximum;

        /// <summary>
        /// Defines the beginning or end of a set of local items.
        /// </summary>
        public Delimeter? Delimeter => (Delimeter?)descriptor.Delimeter;

        internal Report(ReportDescriptorData descriptor)
        {
            this.descriptor = descriptor?.Copy() ?? throw new ArgumentNullException(nameof(descriptor));
        }
    }
}
