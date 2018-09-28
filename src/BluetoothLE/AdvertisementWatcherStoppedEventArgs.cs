namespace Dandy.Devices.BluetoothLE
{
    public class AdvertisementWatcherStoppedEventArgs
    {
        private AdvertisementWatcherError error;

        public AdvertisementWatcherStoppedEventArgs(AdvertisementWatcherError error)
        {
            this.error = error;
        }
    }
}