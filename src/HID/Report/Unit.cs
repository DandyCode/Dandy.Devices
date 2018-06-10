using System;
using System.Reflection;

namespace Dandy.Devices.HID.Report
{
    public struct Unit
    {
        internal const int NibbleBits = 4;

        int value;

        public SystemUnit System {
            get => GetUnit<SystemUnit>();
            set => SetUnit<SystemUnit>(value);
        }

        public LengthUnit Length {
            get => GetUnit<LengthUnit>();
            set => SetUnit<LengthUnit>(value);
        }

        public MassUnit Mass {
            get => GetUnit<MassUnit>();
            set => SetUnit<MassUnit>(value);
        }

        public TimeUnit Time {
            get => GetUnit<TimeUnit>();
            set => SetUnit<TimeUnit>(value);
        }

        public TemperatureUnit Temperature {
            get => GetUnit<TemperatureUnit>();
            set => SetUnit<TemperatureUnit>(value);
        }

        public CurrentUnit Current {
            get => GetUnit<CurrentUnit>();
            set => SetUnit<CurrentUnit>(value);
        }

        public LuminousIntensityUnit LuminousIntensity {
            get => GetUnit<LuminousIntensityUnit>();
            set => SetUnit<LuminousIntensityUnit>(value);
        }

        // TODO: need C# 7.3 for this - then can remove object cast
        private T GetUnit<T>() // where T : Enum
        {
            var attr = typeof(T).GetCustomAttribute<NibbleAttribute>();
            return (T)(object)((value >> (attr.Nibble * NibbleBits)) & 0xF);
        }

        private void SetUnit<T>(T unit) // where T : Enum
        {
            var attr = typeof(T).GetCustomAttribute<NibbleAttribute>();
            value &= ~(0xF << (attr.Nibble * NibbleBits));
            value |= ((int)(object)unit & 0xF) << (attr.Nibble * NibbleBits);
        }

        public static explicit operator int(Unit unit) => unit.value;
        public static explicit operator Unit(int value) => new Unit { value = value };
    }

    [Nibble(0x0)]
    public enum SystemUnit
    {
        None,
        SILinear,
        SIRotatation,
        EnglishLinear,
        EnglishRotation,
        Vendor = 0xF
    }

    [Nibble(0x1)]
    public enum LengthUnit
    {
        None,
        Centimeter,
        Radians,
        Inch,
        Degrees,
    }

    [Nibble(0x2)]
    public enum MassUnit
    {
        None,
        Gram,
        Gram_,
        Slug,
        Slug_,
    }

    [Nibble(0x3)]
    public enum TimeUnit
    {
        None,
        Seconds,
        Seconds_,
        Seconds__,
        Seconds___,
    }

    [Nibble(0x4)]
    public enum TemperatureUnit
    {
        None,
        Kelvin,
        Kelvin_,
        Fahrenheit,
        Fahrenheit_,
    }

    [Nibble(0x5)]
    public enum CurrentUnit
    {
        None,
        Ampere,
        Ampere_,
        Ampere__,
        Ampere___,
    }

    [Nibble(0x6)]
    public enum LuminousIntensityUnit
    {
        None,
        Candella,
        Candella_,
        Candella__,
        Candella___,
    }

    [AttributeUsage(AttributeTargets.Enum)]
    sealed class NibbleAttribute : Attribute
    {
        public int Nibble { get; }

        public NibbleAttribute(int nibble)
        {
            Nibble = nibble;
        }
    }
}
