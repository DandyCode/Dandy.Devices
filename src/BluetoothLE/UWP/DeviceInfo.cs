using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace Dandy.Devices.BluetoothLE
{
    partial class DeviceInfo
    {
        // NOTE: adapters don't seem to return anything - at least on desktop
        const string adapters = "System.Devices.InterfaceClassGuid:=\"{92383B0E-F90E-4AC9-8D44-8C2D0D0EBDA2}\"";
        const string classicDevices = "System.Devices.Aep.ProtocolId:=\"{E0CBF06C-CD8B-4647-BB8A-263B43F0F974}\"";
        const string bleDevices = "System.Devices.Aep.ProtocolId:=\"{BB7BB05E-5972-42B5-94FC-76EAA7084D49}\"";
        static readonly string allDevices = $"{adapters} OR {classicDevices} OR {bleDevices}";

        // available properties: https://msdn.microsoft.com/en-us/library/windows/desktop/ff521659(v=vs.85).aspx
        const string deviceAddress = "System.Devices.Aep.DeviceAddress";
        const string isConnected = "System.Devices.Aep.IsConnected";
        const string isConnectable = "System.Devices.Aep.Bluetooth.Le.IsConnectable";

        private static readonly IEnumerable<string> properties = new List<string> {
            deviceAddress,
            isConnected,
            isConnectable,
        };

        private readonly DeviceInformation info;

        internal DeviceInfo(DeviceInformation info)
        {
            this.info = info ?? throw new ArgumentNullException(nameof(info));
        }

        static DeviceWatcher _CreateWatcher() => new DeviceWatcher(DeviceInformation.CreateWatcher(bleDevices, properties, DeviceInformationKind.AssociationEndpoint));

        static Task<IEnumerable<DeviceInfo>> _FindAllAsync() => DeviceInformation.FindAllAsync(bleDevices, properties, DeviceInformationKind.AssociationEndpoint)
            .AsTask().ContinueWith(t => t.Result.Select(i => new DeviceInfo(i)));

        string _get_Id() => info.Id;

        string _get_Name() => info.Name;

        BluetoothAddress _get_Address() => BluetoothAddress.Parse((string)info.Properties[deviceAddress]);

        bool _get_IsConnected() => (bool)info.Properties[isConnected];

        IReadOnlyList<Guid> _get_ServiceUuids() => throw new NotImplementedException();

        IReadOnlyDictionary<Guid, ReadOnlyMemory<byte>> _get_ServiceData() => throw new NotImplementedException();

        IReadOnlyDictionary<ushort, ReadOnlyMemory<byte>> _get_ManufacturerData() => throw new NotImplementedException();

        short _get_TxPower() => throw new NotImplementedException();

        IReadOnlyDictionary<string, object> _get_Properties() => info.Properties;

        void _Update(DeviceInfoUpdate updateInfo) => info.Update(updateInfo.UpdateInfo);
    }
}
