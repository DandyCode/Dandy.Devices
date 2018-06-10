namespace Dandy.Devices.HID.Report
{
    public sealed class OutputReport : Report
    {
        private readonly int data;

        bool Constant => TestBit(0);
        bool Variable => TestBit(1);
        bool Relative => TestBit(2);
        bool Wrap => TestBit(3);
        bool NonLinear => TestBit(4);
        bool NoPreferred => TestBit(5);
        bool NullState => TestBit(6);
        bool Volatile => TestBit(7);
        bool BufferedBytes => TestBit(8);

        internal OutputReport(int data, ReportDescriptorData descriptor) : base(descriptor)
        {
            this.data = data;
        }

        bool TestBit(int bit) => (data & (1 << bit)) != 0;
    }
}
