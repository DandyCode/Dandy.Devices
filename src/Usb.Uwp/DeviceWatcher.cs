using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Usb;
using Windows.Storage;

namespace Dandy.Devices.Usb.Uwp
{
    public sealed class DeviceWatcher : ObservableBase<IDevice>, IDeviceWatcher
    {
        readonly Subject<IDevice> subject;
        readonly Windows.Devices.Enumeration.DeviceWatcher watcher;

        public DeviceWatcher()
        {
            subject = new Subject<IDevice>();
            const string selector = "System.Devices.InterfaceClassGuid:= \"{A5DCBF10-6530-11D2-901F-00C04FB951ED}\" AND System.Devices.InterfaceEnabled:= System.StructuredQueryType.Boolean#True";
            watcher = DeviceInformation.CreateWatcher(selector);
            watcher.Added += Watcher_Added;
            watcher.Removed += Watcher_Removed;
            watcher.Updated += Watcher_Updated;
            watcher.Stopped += Watcher_Stopped;
        }

        private async void Watcher_Added(Windows.Devices.Enumeration.DeviceWatcher sender, DeviceInformation args)
        {
            // FIXME: async void - need to log error or something
            var device = await UsbDevice.FromIdAsync(args.Id);
            if (device == null) {
                // we probably don't have permission to use it
                return;
            }
            subject.OnNext(new Device(args, device));
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

        protected override IDisposable SubscribeCore(IObserver<IDevice> observer)
        {
            return subject.Subscribe(observer);
        }
    }
}
