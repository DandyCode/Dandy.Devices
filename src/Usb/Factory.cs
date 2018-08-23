using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Dandy.Devices.Usb
{
    /// <summary>
    /// Factory for creating interface instances.
    /// </summary>
    public static partial class Factory
    {
        /// <summary>
        /// Finds all currently attached USB devices.
        /// </summary>
        /// <returns>The list of devices.</returns>
        public static Task<IEnumerable<DeviceInfo>> FindAllAsync() => _FindAllAsync();

        /// <summary>
        /// Finds all currently attached USB devices with the given USB
        /// vendor ID and product ID.
        /// </summary>
        /// <returns>The list of devices.</returns>
        public static Task<IEnumerable<DeviceInfo>> FindAllAsync(ushort vendorId, ushort productId) => _FindAllAsync(vendorId, productId);
    }
}
