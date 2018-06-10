using System;
using System.Collections.Generic;
using System.Linq;
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
    public class DeviceWatcher : ObservableBase<IDevice>, IDeviceWatcher
    {
        readonly Subject<IDevice> subject;
        readonly Context udev;
        Monitor monitor;

        public DeviceWatcher()
        {
            subject = new Subject<IDevice>();
            udev = new Context();
        }

        protected override IDisposable SubscribeCore(IObserver<IDevice> observer)
        {
            return subject.Subscribe(observer);
        }

        /// <summary>
        /// Tries to get an HID device for the udev device.
        /// </summary>
        /// <param name="hidrawUdev">The udev device for an hidraw node</param>
        /// <returns>The HID device or <c>null</c> if we don't have permission to use the hidraw device.</return>
        static IEnumerable<Device> TryGetHidDevices(Dandy.Linux.Udev.Device hidrawUdev)
        {
            try {
                var hidUdev = hidrawUdev.TryGetAncestor("hid");
                var hidraw = new Hidraw(hidrawUdev.DevNode);
                return hidraw.GetReports().Select(x => new Device(x, hidraw));
            }
            catch (UnauthorizedAccessException) {
                return null;
            }
        }
        
        public IDisposable Connect()
        {
            if (monitor != null) {
                throw new InvalidOperationException("already connected");
            }
            monitor = new Monitor(udev);
            monitor.AddMatchSubsystem("hidraw");
            monitor.EnableReceiving();
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            Task.Run(() => {
                try {
                    var enumerator = new Enumerator(udev);
                    enumerator.AddMatchSubsystem("hidraw");
                    enumerator.ScanDevices();
                    foreach (var device in enumerator) {
                        if (token.IsCancellationRequested) {
                            break;
                        }
                        var hids = TryGetHidDevices(device);
                        if (hids == null) {
                            continue;
                        }
                        foreach (var hid in hids) {
                            subject.OnNext(hid);
                        }
                    }
                    var pollfds = new Pollfd[] {
                        new Pollfd { fd = monitor.Fd, events = PollEvents.POLLIN }
                    };
                    while (!token.IsCancellationRequested) {
                        // FIXME: it would be nice if we didn't wake up every 100ms
                        // not sure how to interrupt a system call though
                        // pinvoke pthread_kill() maybe?
                        var ret = Syscall.poll(pollfds, 100);
                        if (ret == -1) {
                            var err = Stdlib.GetLastError();
                            if (err == Errno.EINTR) {
                                continue;
                            }
                            UnixMarshal.ThrowExceptionForError(err);
                        }
                        if (token.IsCancellationRequested) {
                            break;
                        }
                        if (ret == 0) {
                            // timed out
                            continue;
                        }
                        if (pollfds[0].revents.HasFlag(PollEvents.POLLNVAL)) {
                            // monitor file descriptor is closed, monitor must
                            // have been disposed, so complete gracefully
                            break;
                        }
                        var device = monitor.TryReceiveDevice();
                        var hids = TryGetHidDevices(device);
                        if (hids == null) {
                            continue;
                        }
                        foreach (var hid in hids) {
                            subject.OnNext(hid);
                        }
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
