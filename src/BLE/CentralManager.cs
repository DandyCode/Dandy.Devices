using System;
using System.Threading.Tasks;

namespace Dandy.Devices.BLE
{
    public sealed partial class CentralManager
    {
        private CentralManager() { }

        public static partial Task<CentralManager> NewAsync();
    }
}
