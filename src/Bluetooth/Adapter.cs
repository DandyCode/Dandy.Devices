using System.Threading.Tasks;

namespace Dandy.Devices.Bluetooth
{
    /// <summary>
    /// Class that represents a Bluetooth adapter.
    /// </summary>
    public sealed partial class Adapter
    {
        /// <summary>
        /// Gets the Bluetooth address of the adapter.
        /// </summary>
        public BluetoothAddress BluetoothAddress => _get_BluetoothAddress();

        /// <summary>
        /// Gets an instance of a Bluetooth adapter from a platform-specific id.
        /// </summary>
        public static Task<Adapter> FromIdAsync(string id) => _FromIdAsync(id);
    }
}
