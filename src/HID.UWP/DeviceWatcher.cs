using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;

namespace Dandy.Devices.HID.UWP
{
    public sealed class DeviceWatcher : ObservableBase<Device>, IDeviceWatcher
    {
        readonly Subject<Device> subject;
        readonly Windows.Devices.Enumeration.DeviceWatcher watcher;

        public DeviceWatcher()
        {
            subject = new Subject<Device>();
            const string selector = "System.Devices.InterfaceClassGuid:= \"{4D1E55B2-F16F-11CF-88CB-001111000030}\" AND System.Devices.InterfaceEnabled:= System.StructuredQueryType.Boolean#True";
            watcher = DeviceInformation.CreateWatcher(selector);
            watcher.Added += Watcher_Added;
            watcher.Removed += Watcher_Removed;
            watcher.Updated += Watcher_Updated;
            watcher.Stopped += Watcher_Stopped;
        }
        
        private void Watcher_Added(Windows.Devices.Enumeration.DeviceWatcher sender, DeviceInformation args)
        {
            subject.OnNext(new Device(args.Id));
        }

        private void Watcher_Removed(Windows.Devices.Enumeration.DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // TODO: notify devices
        }
        private void Watcher_Updated(Windows.Devices.Enumeration.DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // TODO: notify devices
        }

        private void Watcher_Stopped(Windows.Devices.Enumeration.DeviceWatcher sender, object args)
        {
            subject.OnCompleted();
        }

        public IDisposable Connect()
        {
            watcher.Start();
            return Disposable.Create(() => watcher.Stop());
        }

        public void Dispose()
        {
            watcher.Added -= Watcher_Added;
        }

        protected override IDisposable SubscribeCore(IObserver<Device> observer)
        {
            return subject.Subscribe(observer);
        }
    }
}
