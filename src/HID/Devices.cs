namespace Dandy.Devices.HID
{
    public sealed class Device
    {
        public string Id { get; }

        public Device(string id)
        {
            Id = id;
        }
    }
}
