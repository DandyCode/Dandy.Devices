using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win = Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Dandy.Devices.BluetoothLE
{
    partial class GattService
    {
        private readonly Device device;
        private readonly Win.GattDeviceService service;

        internal GattService(Device device, Win.GattDeviceService service)
        {
            this.device = device ?? throw new ArgumentNullException(nameof(device));
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        Guid _get_Uuid() => service.Uuid;

        Device _get_Device() => device;

        async Task<IReadOnlyList<GattCharacteristic>> _GetCharacteristicsAsync(Guid uuid)
        {
            var result = await service.GetCharacteristicsForUuidAsync(uuid);
            switch (result.Status) {
            case Win.GattCommunicationStatus.Success:
                return result.Characteristics.Select(x => new GattCharacteristic(this, x)).ToList().AsReadOnly();
            case Win.GattCommunicationStatus.AccessDenied:
                throw new AccessViolationException("Access denied");
            case Win.GattCommunicationStatus.ProtocolError:
            case Win.GattCommunicationStatus.Unreachable:
            default:
                throw new Exception("Need a better exception here");
            }
        }

        void _Dispose() => service.Dispose();
    }
}
