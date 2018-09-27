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
        private readonly Win.GattDeviceService service;

        internal GattService(Win.GattDeviceService service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        Guid _get_Uuid() => service.Uuid;

        Task<IReadOnlyList<GattCharacteristic>> _GetCharacteristicsAsync()
        {
            return service.GetCharacteristicsAsync().AsTask().ContinueWith<IReadOnlyList<GattCharacteristic>>(t => {
                switch (t.Result.Status) {
                case Win.GattCommunicationStatus.Success:
                    return t.Result.Characteristics.Select(x => new GattCharacteristic(x)).ToList().AsReadOnly();
                case Win.GattCommunicationStatus.AccessDenied:
                    throw new AccessViolationException("Access denied");
                case Win.GattCommunicationStatus.ProtocolError:
                case Win.GattCommunicationStatus.Unreachable:
                default:
                    throw new Exception("Need a better exception here");
                }
            });
        }

        public void Dispose()
        {

        }
    }
}
