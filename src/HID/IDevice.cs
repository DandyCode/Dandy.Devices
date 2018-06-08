using System;
using System.Collections.Generic;
using System.Text;

namespace Dandy.Devices.HID
{
    public interface IDevice
    {
        /// <summary>
        /// Gets the unique identifier for this device
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets a human-readable name for the device, suitable for display
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the HID usage page for the device
        /// </summary>
        ushort UsagePage { get;  }

        /// <summary>
        /// Gets the HID usage ID for the device
        /// </summary>
        ushort UsageId { get; }

        /// <summary>
        /// Gets the vendor ID of the device
        /// </summary>
        ushort VendorId { get;  }

        /// <summary>
        /// Gets the product ID of the device
        /// </summary>
        ushort ProductId { get;  }

        /// <summary>
        /// Gets the version of the HID device
        /// </summary>
        ushort Version { get;  }
    }
}
