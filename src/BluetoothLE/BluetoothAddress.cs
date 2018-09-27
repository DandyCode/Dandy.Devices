using System;
using System.Globalization;
using System.Linq;

namespace Dandy.Devices.BluetoothLE
{
    /// <summary>
    /// A Bluetooth address. Stored in little-endian binary format.
    /// </summary>
    public struct BluetoothAddress
    {
        byte b0;
        byte b1;
        byte b2;
        byte b3;
        byte b4;
        byte b5;

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0:X2}:{1:X2}:{2:X2}:{3:X2}:{4:X2}:{5:X2}", b5, b4, b3, b2, b1, b0);
        }

        /// <summary>
        /// Parses a string in the format <c>00:00:00:00:00:00</c>
        /// </summary>
        public static BluetoothAddress Parse(string address)
        {
            if (address == null) {
                throw new ArgumentNullException(nameof(address));
            }

            try {
                var parts = address.Split(':');
                if (parts.Length != 6 || parts.Any(p => p.Length != 2)) {
                    throw new ArgumentException();
                }
                return new BluetoothAddress {
                    b0 = byte.Parse(parts[5], NumberStyles.HexNumber),
                    b1 = byte.Parse(parts[4], NumberStyles.HexNumber),
                    b2 = byte.Parse(parts[3], NumberStyles.HexNumber),
                    b3 = byte.Parse(parts[2], NumberStyles.HexNumber),
                    b4 = byte.Parse(parts[1], NumberStyles.HexNumber),
                    b5 = byte.Parse(parts[0], NumberStyles.HexNumber),
                };
            } catch (Exception ex) {
                throw new ArgumentException("Not a valid bluetooth address", nameof(address), ex);
            }
        }

        /// <summary>
        /// Gets a Bluetooth address from a unsigned integer value.
        /// </summary>
        public static BluetoothAddress FromULong(ulong bluetoothAddress)
        {
            var bytes = BitConverter.GetBytes(bluetoothAddress);

            return new BluetoothAddress {
                b0 = bytes[0],
                b1 = bytes[1],
                b2 = bytes[2],
                b3 = bytes[3],
                b4 = bytes[4],
                b5 = bytes[5]
            };
        }

        /// <summary>
        /// Gets a Bluetooth address from a span.
        /// </summary>
        public static BluetoothAddress FromSpan(ReadOnlySpan<byte> span, bool bigEndian = false)
        {
            if (span.Length != 6) {
                throw new ArgumentException("Requires 6 bytes", nameof(span));
            }

            if (bigEndian) {
                return new BluetoothAddress {
                    b0 = span[5],
                    b1 = span[4],
                    b2 = span[3],
                    b3 = span[2],
                    b4 = span[1],
                    b5 = span[0],
                };
            }

            return new BluetoothAddress {
                b0 = span[0],
                b1 = span[1],
                b2 = span[2],
                b3 = span[3],
                b4 = span[4],
                b5 = span[5],
            };
        }
    }
}
