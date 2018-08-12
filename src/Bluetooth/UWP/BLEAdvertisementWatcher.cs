using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;

namespace Dandy.Devices.Bluetooth
{
    partial class BLEAdvertisementWatcher
    {
        private BluetoothLEAdvertisementWatcher watcher;

        static void _BLEAdvertisementWatcher(BLEAdvertisementWatcher instance)
        {
            instance.watcher = new BluetoothLEAdvertisementWatcher();
        }

        void _Start() => watcher.Start();

        void _Stop() => watcher.Stop();

        event EventHandler<BLEAdvertisementReceivedEventArgs> _Received;

        void _add_Received(EventHandler<BLEAdvertisementReceivedEventArgs> value)
        {
            _Received += value;
            if (_Received.GetInvocationList().Length == 1) {
                watcher.Received += Watcher_Received;
            }
        }

        void _remove_Received(EventHandler<BLEAdvertisementReceivedEventArgs> value)
        {
            _Received -= value;
            if (_Received.GetInvocationList().Length == 0) {
                watcher.Received -= Watcher_Received;
            }
        }

        private void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            _Received?.Invoke(this, new BLEAdvertisementReceivedEventArgs(args));
        }

        event EventHandler<BLEAdvertisementStoppedEventArgs> _Stopped;

        void _add_Stopped(EventHandler<BLEAdvertisementStoppedEventArgs> value)
        {
            _Stopped += value;
            if (_Stopped.GetInvocationList().Length == 1) {
                watcher.Stopped += Watcher_Stopped;
            }
        }

        void _remove_Stopped(EventHandler<BLEAdvertisementStoppedEventArgs> value)
        {
            _Stopped -= value;
            if (_Stopped.GetInvocationList().Length == 0) {
                watcher.Stopped -= Watcher_Stopped;
            }
        }

        private void Watcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            _Stopped?.Invoke(this, new BLEAdvertisementStoppedEventArgs(args));
        }
    }
}
