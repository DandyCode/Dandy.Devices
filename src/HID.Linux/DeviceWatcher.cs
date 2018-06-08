using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Dandy.Linux.Udev;
using Mono.Unix;
using Mono.Unix.Native;
using Monitor = Dandy.Linux.Udev.Monitor;

namespace Dandy.Devices.HID.Linux
{
    public class DeviceWatcher : ObservableBase<Device>, IDeviceWatcher
    {
        readonly Subject<Device> subject;
        readonly Context udev;
        Monitor monitor;

        public DeviceWatcher()
        {
            subject = new Subject<Device>();
            udev = new Context();
        }

        protected override IDisposable SubscribeCore(IObserver<Device> observer)
        {
            return subject.Subscribe(observer);
        }
        
        public IDisposable Connect()
        {
            if (monitor != null) {
                throw new InvalidOperationException("already connected");
            }
            monitor = new Monitor(udev);
            monitor.AddMatchSubsystem("hid");
            monitor.EnableReceiving();
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            Task.Run(() => {
                try {
                    var enumerator = new Enumerator(udev);
                    enumerator.AddMatchSubsystem("hid");
                    enumerator.ScanDevices();
                    foreach (var device in enumerator) {
                        if (token.IsCancellationRequested) {
                            break;
                        }
                        subject.OnNext(new Device(device.SysPath));
                    }
                    var pollfds = new Pollfd[] {
                        new Pollfd { fd = monitor.Fd, events = PollEvents.POLLIN }
                    };
                    while (!token.IsCancellationRequested) {
                        // FIXME: it would be nice if we didn't wake up every 100ms
                        // not sure how to interrupt a system call though
                        // pinvoke pthread_kill() maybe?
                        var ret = Syscall.poll(pollfds, 100);
                        if (token.IsCancellationRequested) {
                            break;
                        }
                        if (ret == 0) {
                            // timed out
                            continue;
                        }
                        UnixMarshal.ThrowExceptionForLastErrorIf(ret);
                        if (pollfds[0].revents.HasFlag(PollEvents.POLLNVAL)) {
                            // monitor file descriptor is closed, monitor must
                            // have been disposed, so complete gracefully
                            break;
                        }
                        var device = monitor.TryReceiveDevice();
                        subject.OnNext(new Device(device.SysPath));
                    }
                    subject.OnCompleted();
                }
                catch (Exception ex) {
                    subject.OnError(ex);
                }
                finally {
                    monitor.Dispose();
                    monitor = null;
                }
            }, token);

            return Disposable.Create(() => tokenSource.Cancel());
        }

        public void Dispose()
        {
            monitor?.Dispose();
            udev.Dispose();
        }
    }
}
