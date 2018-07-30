using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;

namespace Dandy.Devices.Usb
{
    /// <summary>
    /// A hot observable for monitoring USB devices.
    /// </summary>
    public interface IDeviceWatcher : IConnectableObservable<IDevice>, IDisposable
    {
    }
}
