using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Dandy.Devices.Serial
{
    /// <summary>
    /// Factory for creating objects
    /// </summary>
    public abstract class Factory
    {
        /// <summary>
        /// Gets a factory instance for the current OS platform via reflection.
        /// </summary>
        /// <returns>The factory</returns>
        public static Factory GetFactoryForOSPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                var assembly = Assembly.Load("Dandy.Devices.Serial.Uwp");
                var type = assembly.GetType("Dandy.Devices.Serial.Uwp.Factory");
                return (Factory)Activator.CreateInstance(type);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                var assembly = Assembly.Load("Dandy.Devices.Serial.Linux");
                var type = assembly.GetType("Dandy.Devices.Serial.Linux.Factory");
                return (Factory)Activator.CreateInstance(type);
            }

            throw new NotSupportedException("The current platform is not supported");
        }

        /// <summary>
        /// Finds all currently attached serial communication devices.
        /// </summary>
        /// <returns>The list of devices.</returns>
        public abstract Task<IEnumerable<DeviceInfo>> FindAllAsync();

        /// <summary>
        /// Finds all currently attached serial communication devices with the give USB
        /// vendor ID and product ID.
        /// </summary>
        /// <returns>The list of devices.</returns>
        public abstract Task<IEnumerable<DeviceInfo>> FindAllAsync(ushort vendorId, ushort productId);
    }
}
