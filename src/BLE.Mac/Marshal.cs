#nullable enable

using System;
using System.Globalization;
using CoreBluetooth;
using Foundation;

namespace Dandy.Devices.BLE.Mac
{
    internal static class Marshal
    {
        public static unsafe NSData MemoryToNSData(ReadOnlyMemory<byte> memory)
        {
            var handle = memory.Pin();
            return new NSData((IntPtr)handle.Pointer, (nuint)memory.Length, (_, _) => handle.Dispose());
        }

        public static Guid CBUuidToGuid(CBUUID uuid)
        {
            var str = uuid.Uuid;

            if (str.Length == 4) {
                return Uuid.From16(ushort.Parse(str, NumberStyles.HexNumber));
            }

            return new(str);
        }
    }
}
