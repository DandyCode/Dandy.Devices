using System;
using System.Threading.Tasks;

using CoreBluetooth;
using CoreFoundation;

namespace Dandy.Devices.BLE
{
    partial class CentralManager
    {
        private CBCentralManager Manager { get; init; }

        public static async partial Task<CentralManager> NewAsync()
        {
            var manager = new CentralManager() {
                Manager = new CBCentralManager(new DispatchQueue("Dandy.Devices.BLE")),
            };
            var source = new TaskCompletionSource();
            manager.Manager.UpdatedState += (s, e) => source.SetResult();
            await source.Task;
            return manager;
        }
    }
}
