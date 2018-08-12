using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;
using Windows.Foundation;

namespace Dandy.Devices.Bluetooth
{
    partial class DeviceWatcher
    {
        private readonly Windows.Devices.Enumeration.DeviceWatcher watcher;

        internal DeviceWatcher(Windows.Devices.Enumeration.DeviceWatcher watcher)
        {
            this.watcher = watcher ?? throw new ArgumentNullException(nameof(watcher));
        }

        event EventHandler<DeviceInfo> _Added;

        void _add_Added(EventHandler<DeviceInfo> value)
        {
            _Added += value;
            if (_Added.GetInvocationList().Length == 1) {
                watcher.Added += OnAdded;
            }
        }

        void _remove_Added(EventHandler<DeviceInfo> value)
        {
            _Added -= value;
            if (_Added.GetInvocationList().Length == 0) {
                watcher.Added -= OnAdded;
            }
        }

        void OnAdded(Windows.Devices.Enumeration.DeviceWatcher watcher, DeviceInformation info)
        {
            _Added?.Invoke(this, new DeviceInfo(info));
        }

        event EventHandler<DeviceInfoUpdate> _Updated;

        void _add_Updated(EventHandler<DeviceInfoUpdate> value)
        {
            _Updated += value;
            if (_Updated.GetInvocationList().Length == 1) {
                watcher.Updated += OnUpdated;
            }
        }

        void _remove_Updated(EventHandler<DeviceInfoUpdate> value)
        {
            _Updated -= value;
            if (_Updated.GetInvocationList().Length == 0) {
                watcher.Updated -= OnUpdated;
            }
        }

        void OnUpdated(Windows.Devices.Enumeration.DeviceWatcher watcher, DeviceInformationUpdate info)
        {
            _Updated?.Invoke(this, new DeviceInfoUpdate(info));
        }

        event EventHandler<DeviceInfoUpdate> _Removed;

        void _add_Removed(EventHandler<DeviceInfoUpdate> value)
        {
            _Removed += value;
            if (_Removed.GetInvocationList().Length == 1) {
                watcher.Removed += OnRemoved;
            }
        }

        void _remove_Removed(EventHandler<DeviceInfoUpdate> value)
        {
            _Removed -= value;
            if (_Removed.GetInvocationList().Length == 0) {
                watcher.Removed -= OnRemoved;
            }
        }

        void OnRemoved(Windows.Devices.Enumeration.DeviceWatcher watcher, DeviceInformationUpdate info)
        {
            _Removed?.Invoke(this, new DeviceInfoUpdate(info));
        }

        event EventHandler _EnumerationCompleted;

        void _add_EnumerationCompleted(EventHandler value)
        {
            _EnumerationCompleted += value;
            if (_EnumerationCompleted.GetInvocationList().Length == 1) {
                watcher.EnumerationCompleted += OnEnumerationCompleted;
            }
        }

        void _remove_EnumerationCompleted(EventHandler value)
        {
            _EnumerationCompleted -= value;
            if (_EnumerationCompleted.GetInvocationList().Length == 0) {
                watcher.EnumerationCompleted -= OnEnumerationCompleted;
            }
        }

        void OnEnumerationCompleted(Windows.Devices.Enumeration.DeviceWatcher watcher, object obj)
        {
            _EnumerationCompleted?.Invoke(this, EventArgs.Empty);
        }

        event EventHandler _Stopped;

        void _add_Stopped(EventHandler value)
        {
            _Stopped += value;
            if (_Stopped.GetInvocationList().Length == 1) {
                watcher.Stopped += OnStopped;
            }
        }

        void _remove_Stopped(EventHandler value)
        {
            _Stopped -= value;
            if (_Stopped.GetInvocationList().Length == 0) {
                watcher.Stopped -= OnStopped;
            }
        }

        void OnStopped(Windows.Devices.Enumeration.DeviceWatcher watcher, object obj)
        {
            _Stopped?.Invoke(this, EventArgs.Empty);
        }

        void _Start() => watcher.Start();

        void _Stop() => watcher.Stop();
    }
}
