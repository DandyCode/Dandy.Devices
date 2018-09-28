using System;
using System.Collections.Generic;

namespace Dandy.Devices.BluetoothLE
{
    /// <summary>
    /// Class used to scan for Bluetooth Low Energy device advertisements.
    /// </summary>
    public sealed partial class AdvertisementWatcher
    {
        const string bleDevices = "System.Devices.Aep.ProtocolId:=\"{BB7BB05E-5972-42B5-94FC-76EAA7084D49}\"";

        // available properties: https://msdn.microsoft.com/en-us/library/windows/desktop/ff521659(v=vs.85).aspx
        const string deviceAddress = "System.Devices.Aep.DeviceAddress";
        const string isConnected = "System.Devices.Aep.IsConnected";
        const string isConnectable = "System.Devices.Aep.Bluetooth.Le.IsConnectable";

        private static readonly IEnumerable<string> properties = new List<string> {
            deviceAddress,
            isConnected,
            isConnectable,
        };
 
        /// <summary>
        /// Triggered when a new advertisment is received.
        /// </summary>
        public event EventHandler<AdvertisementReceivedEventArgs> Received;

        void OnReceived(Advertisement advertisement, BluetoothAddress address, short rssi)
        {
            Received?.Invoke(this, new AdvertisementReceivedEventArgs(advertisement, address, rssi));
        }

        /// <summary>
        /// Triggered when scanning has stopped.
        /// </summary>
        public event EventHandler<AdvertisementWatcherStoppedEventArgs> Stopped;

        void OnStopped(AdvertisementWatcherError error)
        {
            Stopped?.Invoke(this, new AdvertisementWatcherStoppedEventArgs(error));
        }

        /// <summary>
        /// Create a new advertisement watcher.
        /// </summary>
        public AdvertisementWatcher() : this((object)null)
        {
        }

        /// <summary>
        /// Create a new advertisement watcher.
        /// </summary>
        public AdvertisementWatcher(params Guid[] serviceUuids) : this((object)serviceUuids)
        {
        }

        /// <summary>
        /// Start scanning for advertisements.
        /// </summary>
        public void Start() => _Start();

        /// <summary>
        /// Stops scanning.
        /// </summary>
        public void Stop() => _Stop();
    }
}
