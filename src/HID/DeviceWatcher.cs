using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Dandy.Devices.HID
{
    public static class DeviceWatcher
    {
        /// <summary>
        /// Gets an HID device watcher for the current platform.
        /// </summary>
        public static IDeviceWatcher ForPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                var assembly = Assembly.Load("Dandy.Devices.HID.UWP");
                var type = assembly.GetType("Dandy.Devices.HID.UWP.DeviceWatcher");
                return (IDeviceWatcher)Activator.CreateInstance(type);
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                var assembly = Assembly.Load("Dandy.Devices.HID.Linux");
                var type = assembly.GetType("Dandy.Devices.HID.Linux.DeviceWatcher");
                return (IDeviceWatcher)Activator.CreateInstance(type);
            }

            throw new NotSupportedException("The current platform is not supported");
        }
    }
}
