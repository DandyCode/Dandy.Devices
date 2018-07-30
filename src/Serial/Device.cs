using System;
using System.IO;

namespace Dandy.Devices.Serial
{
    /// <summary>
    /// Abstract class that represents a serial communication device.
    /// </summary>
    public abstract class Device : IDisposable
    {
        /// <summary>
        /// Gets and sets the baud rate of the device.
        /// </summary>
        public abstract uint BaudRate { get; set; }

        /// <summary>
        /// Input stream for reading from the device.
        /// </summary>
        public abstract Stream InputStream { get; }

        /// <summary>
        /// Output stream for writing to the device.
        /// </summary>
        public abstract Stream OutputStream { get; }

        /// <summary>
        /// Close the I/O connection to the device.
        /// </summary>
        public abstract void Dispose();
    }
}
