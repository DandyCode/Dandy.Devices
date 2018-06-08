using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Dandy.Devices.HID
{
    public interface IPlatformDeviceWatcher : IDisposable
    {
        void Start();

        void Stop();

        event EventHandler<DeviceEventArgs> Added;
        event EventHandler<DeviceEventArgs> Changed;
        event EventHandler<DeviceEventArgs> Removed;
        event EventHandler<EventArgs> Stopped;
    }

    public sealed class DeviceEventArgs : EventArgs
    {
        public string Id { get; }

        public DeviceEventArgs(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }
    }

    public sealed class DeviceWatcher : ObservableBase<Device>, IConnectableObservable<Device>
    {
        /// <summary>
        /// Gets an HID device watcher for the current platform.
        /// </summary>
        public static IDeviceWatcher ForPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                var assembly = Assembly.Load("Dandy.Devices.HID.Linux");
                var type = assembly.GetType("Dandy.Devices.HID.Linux.DeviceWatcher");
                return (IDeviceWatcher)Activator.CreateInstance(type);
            }

            throw new NotSupportedException("The current platform is not supported");
        }

        readonly IPlatformDeviceWatcher watcher;
        readonly IObservable<Device> addedObservable;
        readonly IObservable<DeviceEventArgs> changedObservable;
        readonly IObservable<DeviceEventArgs> removedObservable;
        readonly IObservable<EventArgs> stoppedObservable;

        public DeviceWatcher(IPlatformDeviceWatcher watcher)
        {
            this.watcher = watcher ?? throw new ArgumentNullException(nameof(watcher));
            removedObservable = Observable.FromEvent<EventHandler<DeviceEventArgs>, DeviceEventArgs>(
                x => watcher.Added += x, x => watcher.Added -= x
            );
        }

        public IDisposable Connect()
        {
            watcher.Start();
            return Disposable.Create(() => watcher.Stop());
        }

        protected override IDisposable SubscribeCore(IObserver<Device> observer)
        {
            var notifier = observer.ToNotifier();

            watcher.Added += (s, e) => {
                var device = new Device(e.Id);
                notifier(Notification.CreateOnNext(device));
                removedObservable.Where(x => x.Id == e.Id);
            };

            return null;
        }

        public void Dispose() => watcher.Dispose();
    }
}
