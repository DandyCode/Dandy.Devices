using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dandy.Devices.Bluetooth
{
    /// <summary>
    /// Class used to monitor Bluetooth Low Energy advertisements
    /// </summary>
    public sealed partial class BLEAdvertisementWatcher
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        public BLEAdvertisementWatcher() => _BLEAdvertisementWatcher(this);

        /// <summary>
        /// Starts the watcher.
        /// </summary>
        public void Start() => _Start();

        /// <summary>
        /// Stops the watcher.
        /// </summary>
        public void Stop() => _Stop();

        /// <summary>
        /// Event triggered when an advertisement is received.
        /// </summary>
        public event EventHandler<BLEAdvertisementReceivedEventArgs> Received {
            add => _add_Received(value);
            remove => _remove_Received(value);
        }

        /// <summary>
        /// Event triggered when the watcher has stopped.
        /// </summary>
        public event EventHandler<BLEAdvertisementStoppedEventArgs> Stopped {
            add => _add_Stopped(value);
            remove => _remove_Stopped(value);
        }
    }
}
