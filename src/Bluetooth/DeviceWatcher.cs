using System;

namespace Dandy.Devices.Bluetooth
{
    public sealed partial class DeviceWatcher
    {
        public event EventHandler<DeviceInfo> Added {
            add => _add_Added(value);
            remove => _remove_Added(value);
        }

        public event EventHandler<DeviceInfoUpdate> Updated {
            add => _add_Updated(value);
            remove => _remove_Updated(value);
        }

        public event EventHandler<DeviceInfoUpdate> Removed {
            add => _add_Removed(value);
            remove => _remove_Removed(value);
        }

        public event EventHandler EnumerationCompleted {
            add => _add_EnumerationCompleted(value);
            remove => _remove_EnumerationCompleted(value);
        }

        public event EventHandler Stopped {
            add => _add_Stopped(value);
            remove => _remove_Stopped(value);
        }

        public void Start() => _Start();

        public void Stop() => _Stop();
    }
}
