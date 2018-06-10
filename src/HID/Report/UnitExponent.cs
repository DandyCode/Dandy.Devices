using System;
using System.Reflection;

namespace Dandy.Devices.HID.Report
{
    public struct UnitExponent
    {
        int value;

        public int System {
            get => GetExponent<SystemUnit>();
            set => SetExponent<SystemUnit>(value);
        }

        public int Length {
            get => GetExponent<LengthUnit>();
            set => SetExponent<LengthUnit>(value);
        }

        public int Mass {
            get => GetExponent<MassUnit>();
            set => SetExponent<MassUnit>(value);
        }

        public int Time {
            get => GetExponent<TimeUnit>();
            set => SetExponent<TimeUnit>(value);
        }

        public int Temperature {
            get => GetExponent<TemperatureUnit>();
            set => SetExponent<TemperatureUnit>(value);
        }

        public int Current {
            get => GetExponent<CurrentUnit>();
            set => SetExponent<CurrentUnit>(value);
        }

        public int LuminousIntensity {
            get => GetExponent<LuminousIntensityUnit>();
            set => SetExponent<LuminousIntensityUnit>(value);
        }

        // TODO: need C# 7.3 for this - then can remove object cast
        private int GetExponent<T>() // where T : Enum
        {
            var attr = typeof(T).GetCustomAttribute<NibbleAttribute>();
            var ret = (value >> (attr.Nibble * Unit.NibbleBits)) & 0xF;
            if (ret >= 0x8) {
                ret -= 16;
            }
            return ret;
        }

        private void SetExponent<T>(int exponent) // where T : Enum
        {
            if (exponent >= 0x8 || exponent < -0x8) {
                throw new ArgumentOutOfRangeException(nameof(exponent));
            }
            var attr = typeof(T).GetCustomAttribute<NibbleAttribute>();
            value &= ~(0xF << (attr.Nibble * Unit.NibbleBits));
            value |= (exponent & 0xF) << (attr.Nibble * Unit.NibbleBits);
        }

        public static explicit operator int(UnitExponent exponent) => exponent.value;

        public static explicit operator UnitExponent(int value) => new UnitExponent { value = value };
    }
}
