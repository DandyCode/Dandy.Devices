using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using bluez.DBus;
using Tmds.DBus;

namespace Dandy.Devices.Bluetooth
{
    partial class GattCharacteristic
    {
        private readonly IGattCharacteristic1 proxy;
        private readonly IDictionary<string, object> properties;
        private IDisposable propertyWatcher;

        GattCharacteristic(ObjectPath path)
        {
            proxy = Connection.System.CreateProxy<IGattCharacteristic1>("org.bluez", path);
            properties = new Dictionary<string, object>();
        }

        internal static async Task<GattCharacteristic> CreateInstanceAsync(ObjectPath path)
        {
            var instance = new GattCharacteristic(path);
            instance.propertyWatcher = await instance.proxy.WatchPropertiesAsync(x => instance.HandlePropertyChanges(x.Changed));
            instance.HandlePropertyChanges(await instance.proxy.GetAllAsync());

            return instance;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            propertyWatcher?.Dispose();
        }

        private void HandlePropertyChanges(IEnumerable<KeyValuePair<string, object>> changes)
        {
            foreach (var p in changes) {
                properties[p.Key] = p.Value;
                if (p.Key == "Value") {
                    _ValueChanged?.Invoke(this, new GattValueChangedEventArgs((byte[])p.Value));
                }
            }
        }

        Guid _get_Uuid() => new Guid((string)properties["UUID"]);

        [DllImport("c", SetLastError = true)]
        unsafe static extern IntPtr write(int fd, void *buf, UIntPtr count);

        [DllImport("c")]
        static extern IntPtr strerror(int errnum);

        const int EINTR = 4;
        const int EAGAIN = 11;

        async Task _WriteValueAsync(Memory<byte> data, GattWriteOption option) {
            if (option == GattWriteOption.WriteWithoutResponse) {
                var (fd, mtu) = await proxy.AcquireWriteAsync(new Dictionary<string, object>());
                // do low level file I/O in background task
                await Task.Run(() => {
                    try {
                        using (var dataHandle = data.Pin()) {
                            unsafe {
                                var written = 0;
                                while (written < data.Length) {
                                    var ret = write((int)fd.DangerousGetHandle(), dataHandle.Pointer, (UIntPtr)data.Length);
                                    if ((int)ret == -1) {
                                        var err = Marshal.GetLastWin32Error();
                                        if (err == EINTR || err == EAGAIN) {
                                            continue;
                                        }
                                        // TODO: marshal other errors to more specific exceptions
                                        throw new Exception($"IO error: {Marshal.PtrToStringAnsi(strerror(err))}");
                                    }
                                    written += (int)ret;
                                }
                            }
                        }
                    }
                    finally {
                        fd.Close();
                    }
                });
            }
            else {
                if (data.Length <= 20) {
                    await proxy.WriteValueAsync(data.ToArray(), new Dictionary<string, object>());
                }
                else {
                    var options = new Dictionary<string, object> { { "offset", (ushort)0 } };
                    for (ushort i = 0; i < data.Length; i += 20) {
                        options["offset"] = i;
                        await proxy.WriteValueAsync(data.Slice(i, Math.Min(data.Length - i, 20)).ToArray(), options);
                    }
                }
            }
        }

        Task<Memory<byte>> _ReadValueAsync() => proxy.ReadValueAsync(new Dictionary<string, object>()).ContinueWith(t => t.Result.AsMemory());

        event EventHandler<GattValueChangedEventArgs> _ValueChanged;

        Task _StartNotifyAsync() => proxy.StartNotifyAsync(); // TODO: might want to wait until "Notifying" property is true

        Task _StopNotifyAsync() => proxy.StopNotifyAsync();

        void _add_ValueChanged(EventHandler<GattValueChangedEventArgs> handler) => _ValueChanged += handler;
        void _remove_ValueChanged(EventHandler<GattValueChangedEventArgs> handler) => _ValueChanged -= handler;
    }
}
