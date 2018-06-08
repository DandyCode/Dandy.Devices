using System;
using System.Reactive.Subjects;

namespace Dandy.Devices.HID
{
    public interface IDeviceWatcher : IConnectableObservable<Device>, IDisposable
    {
    }
}
