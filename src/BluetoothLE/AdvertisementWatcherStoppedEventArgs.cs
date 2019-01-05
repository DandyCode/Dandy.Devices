namespace Dandy.Devices.BluetoothLE
{
    public class AdvertisementWatcherStoppedEventArgs
    {
        public AdvertisementWatcherError Error { get; }

        public AdvertisementWatcherStoppedEventArgs(AdvertisementWatcherError error)
        {
            Error = error;
        }
    }
}
