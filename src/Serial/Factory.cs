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
    public static class Factory
    {
        public static Task<IEnumerable<IDeviceInfo>> FindAllAsync()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                var assembly = Assembly.Load("Dandy.Devices.Serial.Uwp");
                var type = assembly.GetType("Dandy.Devices.Serial.Uwp.DeviceInfo");
                var method = type.GetMethod("FindAllAsync", BindingFlags.Public | BindingFlags.Static);
                return (Task<IEnumerable<IDeviceInfo>>)method.Invoke(null, null);
            }

            throw new NotSupportedException("The current platform is not supported");
        }
    }
}
