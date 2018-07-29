using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Dandy.Devices.Usb
{
    /// <summary>
    /// Factory for creating interface instances.
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Create a device watcher for detecting USB hotplug events.
        /// </summary>
        /// <returns>A new device watcher</returns>
        public static IDeviceWatcher CreateDeviceWatcher()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                var assembly = Assembly.Load("Dandy.Devices.Usb.Uwp");
                var type = assembly.GetType("Dandy.Devices.Usb.Uwp.DeviceWatcher");
                return (IDeviceWatcher)Activator.CreateInstance(type);
            }

            throw new NotSupportedException("The current platform is not supported");
        }
    }
}
