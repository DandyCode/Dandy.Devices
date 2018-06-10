using System;
using Dandy.Devices.HID.Report;
using Xunit;

namespace Dandy.Devices.HIDTests
{
    public class UnitTests
    {
        [Fact]
        public void TestSystemUnit()
        {
            var unit = new Unit { System = (SystemUnit)0xF };
            Assert.Equal((SystemUnit)0xF, unit.System);
            Assert.Equal(0x0000000F, (int)unit);
        }

        [Fact]
        public void TestLengthUnit()
        {
            var unit = new Unit { Length = (LengthUnit)0xF };
            Assert.Equal((LengthUnit)0xF, unit.Length);
            Assert.Equal(0x000000F0, (int)unit);
        }

        [Fact]
        public void TestMassUnit()
        {
            var unit = new Unit { Mass = (MassUnit)0xF };
            Assert.Equal((MassUnit)0xF, unit.Mass);
            Assert.Equal(0x00000F00, (int)unit);
        }

        [Fact]
        public void TestTimeUnit()
        {
            var unit = new Unit { Time = (TimeUnit)0xF };
            Assert.Equal((TimeUnit)0xF, unit.Time);
            Assert.Equal(0x0000F000, (int)unit);
        }

        [Fact]
        public void TestTemperatureUnit()
        {
            var unit = new Unit { Temperature = (TemperatureUnit)0xF };
            Assert.Equal((TemperatureUnit)0xF, unit.Temperature);
            Assert.Equal(0x000F0000, (int)unit);
        }

        [Fact]
        public void TestCurrentUnit()
        {
            var unit = new Unit { Current = (CurrentUnit)0xF };
            Assert.Equal((CurrentUnit)0xF, unit.Current);
            Assert.Equal(0x00F00000, (int)unit);
        }

        [Fact]
        public void TestLuminousIntensityUnit()
        {
            var unit = new Unit { LuminousIntensity = (LuminousIntensityUnit)0xF };
            Assert.Equal((LuminousIntensityUnit)0xF, unit.LuminousIntensity);
            Assert.Equal(0x0F000000, (int)unit);
        }

        [Fact]
        public void TestSystemExponent()
        {
            for (int i = -8; i < 8; i++) {
                var exp = new UnitExponent { System = i };
                Assert.Equal(i, exp.System);
                Assert.Equal(0, (int)exp & 0xFFFFFFF0);
            }
        }

        [Fact]
        public void TestLengtExponenth()
        {
            for (int i = -8; i < 8; i++) {
                var exp = new UnitExponent { Length = i };
                Assert.Equal(i, exp.Length);
                Assert.Equal(0, (int)exp & 0xFFFFFF0F);
            }
        }

        [Fact]
        public void TestMassExponent()
        {
            for (int i = -8; i < 8; i++) {
                var exp = new UnitExponent { Mass = i };
                Assert.Equal(i, exp.Mass);
                Assert.Equal(0, (int)exp & 0xFFFFF0FF);
            }
        }

        [Fact]
        public void TestTimeExponent()
        {
            for (int i = -8; i < 8; i++) {
                var exp = new UnitExponent { Time = i };
                Assert.Equal(i, exp.Time);
                Assert.Equal(0, (int)exp & 0xFFFF0FFF);
            }
        }

        [Fact]
        public void TestTemperatureExponent()
        {
            for (int i = -8; i < 8; i++) {
                var exp = new UnitExponent { Temperature = i };
                Assert.Equal(i, exp.Temperature);
                Assert.Equal(0, (int)exp & 0xFFF0FFFF);
            }
        }

        [Fact]
        public void TestCurrentExponent()
        {
            for (int i = -8; i < 8; i++) {
                var exp = new UnitExponent { Current = i };
                Assert.Equal(i, exp.Current);
                Assert.Equal(0, (int)exp & 0xFF0FFFFF);
            }
        }

        [Fact]
        public void TestLuminousIntensityExponent()
        {
            for (int i = -8; i < 8; i++) {
                var exp = new UnitExponent { LuminousIntensity = i };
                Assert.Equal(i, exp.LuminousIntensity);
                Assert.Equal(0, (int)exp & 0xF0FFFFFF);
            }
        }
    }
}
