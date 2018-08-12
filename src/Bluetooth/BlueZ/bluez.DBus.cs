using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tmds.DBus;

[assembly: InternalsVisibleTo(Tmds.DBus.Connection.DynamicAssemblyName)]
namespace bluez.DBus
{
    [DBusInterface("org.freedesktop.DBus.ObjectManager")]
    interface IObjectManager : IDBusObject
    {
        Task<IDictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>>> GetManagedObjectsAsync();
        Task<IDisposable> WatchInterfacesAddedAsync(Action<(ObjectPath @object, IDictionary<string, IDictionary<string, object>> interfaces)> handler, Action<Exception> onError = null);
        Task<IDisposable> WatchInterfacesRemovedAsync(Action<(ObjectPath @object, string[] interfaces)> handler, Action<Exception> onError = null);
    }

    [DBusInterface("org.bluez.AgentManager1")]
    interface IAgentManager1 : IDBusObject
    {
        Task RegisterAgentAsync(ObjectPath Agent, string Capability);
        Task UnregisterAgentAsync(ObjectPath Agent);
        Task RequestDefaultAgentAsync(ObjectPath Agent);
    }

    [DBusInterface("org.bluez.ProfileManager1")]
    interface IProfileManager1 : IDBusObject
    {
        Task RegisterProfileAsync(ObjectPath Profile, string UUID, IDictionary<string, object> Options);
        Task UnregisterProfileAsync(ObjectPath Profile);
    }

    [DBusInterface("org.bluez.Adapter1")]
    interface IAdapter1 : IDBusObject
    {
        Task StartDiscoveryAsync();
        Task SetDiscoveryFilterAsync(IDictionary<string, object> Properties);
        Task StopDiscoveryAsync();
        Task RemoveDeviceAsync(ObjectPath Device);
        Task<string[]> GetDiscoveryFiltersAsync();
        Task<T> GetAsync<T>(string prop);
        Task<IDictionary<string, object>> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }

    static class Adapter1Extensions
    {
        public static Task<string> GetAddressAsync(this IAdapter1 o) => o.GetAsync<string>("Address");
        public static Task<string> GetAddressTypeAsync(this IAdapter1 o) => o.GetAsync<string>("AddressType");
        public static Task<string> GetNameAsync(this IAdapter1 o) => o.GetAsync<string>("Name");
        public static Task<string> GetAliasAsync(this IAdapter1 o) => o.GetAsync<string>("Alias");
        public static Task<uint> GetClassAsync(this IAdapter1 o) => o.GetAsync<uint>("Class");
        public static Task<bool> GetPoweredAsync(this IAdapter1 o) => o.GetAsync<bool>("Powered");
        public static Task<bool> GetDiscoverableAsync(this IAdapter1 o) => o.GetAsync<bool>("Discoverable");
        public static Task<uint> GetDiscoverableTimeoutAsync(this IAdapter1 o) => o.GetAsync<uint>("DiscoverableTimeout");
        public static Task<bool> GetPairableAsync(this IAdapter1 o) => o.GetAsync<bool>("Pairable");
        public static Task<uint> GetPairableTimeoutAsync(this IAdapter1 o) => o.GetAsync<uint>("PairableTimeout");
        public static Task<bool> GetDiscoveringAsync(this IAdapter1 o) => o.GetAsync<bool>("Discovering");
        public static Task<string[]> GetUUIDsAsync(this IAdapter1 o) => o.GetAsync<string[]>("UUIDs");
        public static Task<string> GetModaliasAsync(this IAdapter1 o) => o.GetAsync<string>("Modalias");
        public static Task SetAliasAsync(this IAdapter1 o, string val) => o.SetAsync("Alias", val);
        public static Task SetPoweredAsync(this IAdapter1 o, bool val) => o.SetAsync("Powered", val);
        public static Task SetDiscoverableAsync(this IAdapter1 o, bool val) => o.SetAsync("Discoverable", val);
        public static Task SetDiscoverableTimeoutAsync(this IAdapter1 o, uint val) => o.SetAsync("DiscoverableTimeout", val);
        public static Task SetPairableAsync(this IAdapter1 o, bool val) => o.SetAsync("Pairable", val);
        public static Task SetPairableTimeoutAsync(this IAdapter1 o, uint val) => o.SetAsync("PairableTimeout", val);
    }

    [DBusInterface("org.bluez.GattManager1")]
    interface IGattManager1 : IDBusObject
    {
        Task RegisterApplicationAsync(ObjectPath Application, IDictionary<string, object> Options);
        Task UnregisterApplicationAsync(ObjectPath Application);
    }

    [DBusInterface("org.bluez.LEAdvertisingManager1")]
    interface ILEAdvertisingManager1 : IDBusObject
    {
        Task RegisterAdvertisementAsync(ObjectPath Advertisement, IDictionary<string, object> Options);
        Task UnregisterAdvertisementAsync(ObjectPath Service);
        Task<T> GetAsync<T>(string prop);
        Task<LEAdvertisingManager1Properties> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }

    [Dictionary]
    class LEAdvertisingManager1Properties
    {
        private byte _ActiveInstances = default (byte);
        public byte ActiveInstances
        {
            get
            {
                return _ActiveInstances;
            }

            set
            {
                _ActiveInstances = (value);
            }
        }

        private byte _SupportedInstances = default (byte);
        public byte SupportedInstances
        {
            get
            {
                return _SupportedInstances;
            }

            set
            {
                _SupportedInstances = (value);
            }
        }

        private string[] _SupportedIncludes = default (string[]);
        public string[] SupportedIncludes
        {
            get
            {
                return _SupportedIncludes;
            }

            set
            {
                _SupportedIncludes = (value);
            }
        }
    }

    static class LEAdvertisingManager1Extensions
    {
        public static Task<byte> GetActiveInstancesAsync(this ILEAdvertisingManager1 o) => o.GetAsync<byte>("ActiveInstances");
        public static Task<byte> GetSupportedInstancesAsync(this ILEAdvertisingManager1 o) => o.GetAsync<byte>("SupportedInstances");
        public static Task<string[]> GetSupportedIncludesAsync(this ILEAdvertisingManager1 o) => o.GetAsync<string[]>("SupportedIncludes");
    }

    [DBusInterface("org.bluez.Media1")]
    interface IMedia1 : IDBusObject
    {
        Task RegisterEndpointAsync(ObjectPath Endpoint, IDictionary<string, object> Properties);
        Task UnregisterEndpointAsync(ObjectPath Endpoint);
        Task RegisterPlayerAsync(ObjectPath Player, IDictionary<string, object> Properties);
        Task UnregisterPlayerAsync(ObjectPath Player);
    }

    [DBusInterface("org.bluez.NetworkServer1")]
    interface INetworkServer1 : IDBusObject
    {
        Task RegisterAsync(string Uuid, string Bridge);
        Task UnregisterAsync(string Uuid);
    }

    [DBusInterface("org.bluez.Device1")]
    interface IDevice1 : IDBusObject
    {
        Task DisconnectAsync();
        Task ConnectAsync();
        Task ConnectProfileAsync(string UUID);
        Task DisconnectProfileAsync(string UUID);
        Task PairAsync();
        Task CancelPairingAsync();
        Task<T> GetAsync<T>(string prop);
        Task<IDictionary<string, object>> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }

    static class Device1Extensions
    {
        public static Task<string> GetAddressAsync(this IDevice1 o) => o.GetAsync<string>("Address");
        public static Task<string> GetAddressTypeAsync(this IDevice1 o) => o.GetAsync<string>("AddressType");
        public static Task<string> GetNameAsync(this IDevice1 o) => o.GetAsync<string>("Name");
        public static Task<string> GetAliasAsync(this IDevice1 o) => o.GetAsync<string>("Alias");
        public static Task<uint> GetClassAsync(this IDevice1 o) => o.GetAsync<uint>("Class");
        public static Task<ushort> GetAppearanceAsync(this IDevice1 o) => o.GetAsync<ushort>("Appearance");
        public static Task<string> GetIconAsync(this IDevice1 o) => o.GetAsync<string>("Icon");
        public static Task<bool> GetPairedAsync(this IDevice1 o) => o.GetAsync<bool>("Paired");
        public static Task<bool> GetTrustedAsync(this IDevice1 o) => o.GetAsync<bool>("Trusted");
        public static Task<bool> GetBlockedAsync(this IDevice1 o) => o.GetAsync<bool>("Blocked");
        public static Task<bool> GetLegacyPairingAsync(this IDevice1 o) => o.GetAsync<bool>("LegacyPairing");
        public static Task<short> GetRSSIAsync(this IDevice1 o) => o.GetAsync<short>("RSSI");
        public static Task<bool> GetConnectedAsync(this IDevice1 o) => o.GetAsync<bool>("Connected");
        public static Task<string[]> GetUUIDsAsync(this IDevice1 o) => o.GetAsync<string[]>("UUIDs");
        public static Task<string> GetModaliasAsync(this IDevice1 o) => o.GetAsync<string>("Modalias");
        public static Task<ObjectPath> GetAdapterAsync(this IDevice1 o) => o.GetAsync<ObjectPath>("Adapter");
        public static Task<IDictionary<ushort, object>> GetManufacturerDataAsync(this IDevice1 o) => o.GetAsync<IDictionary<ushort, object>>("ManufacturerData");
        public static Task<IDictionary<string, object>> GetServiceDataAsync(this IDevice1 o) => o.GetAsync<IDictionary<string, object>>("ServiceData");
        public static Task<short> GetTxPowerAsync(this IDevice1 o) => o.GetAsync<short>("TxPower");
        public static Task<bool> GetServicesResolvedAsync(this IDevice1 o) => o.GetAsync<bool>("ServicesResolved");
        public static Task SetAliasAsync(this IDevice1 o, string val) => o.SetAsync("Alias", val);
        public static Task SetTrustedAsync(this IDevice1 o, bool val) => o.SetAsync("Trusted", val);
        public static Task SetBlockedAsync(this IDevice1 o, bool val) => o.SetAsync("Blocked", val);
    }

    [DBusInterface("org.bluez.Input1")]
    interface IInput1 : IDBusObject
    {
        Task<T> GetAsync<T>(string prop);
        Task<Input1Properties> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }

    [Dictionary]
    class Input1Properties
    {
        private string _ReconnectMode = default (string);
        public string ReconnectMode
        {
            get
            {
                return _ReconnectMode;
            }

            set
            {
                _ReconnectMode = (value);
            }
        }
    }

    static class Input1Extensions
    {
        public static Task<string> GetReconnectModeAsync(this IInput1 o) => o.GetAsync<string>("ReconnectMode");
    }

    [DBusInterface("org.bluez.MediaControl1")]
    interface IMediaControl1 : IDBusObject
    {
        Task PlayAsync();
        Task PauseAsync();
        Task StopAsync();
        Task NextAsync();
        Task PreviousAsync();
        Task VolumeUpAsync();
        Task VolumeDownAsync();
        Task FastForwardAsync();
        Task RewindAsync();
        Task<T> GetAsync<T>(string prop);
        Task<MediaControl1Properties> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }

    [Dictionary]
    class MediaControl1Properties
    {
        private bool _Connected = default (bool);
        public bool Connected
        {
            get
            {
                return _Connected;
            }

            set
            {
                _Connected = (value);
            }
        }

        private ObjectPath _Player = default (ObjectPath);
        public ObjectPath Player
        {
            get
            {
                return _Player;
            }

            set
            {
                _Player = (value);
            }
        }
    }

    static class MediaControl1Extensions
    {
        public static Task<bool> GetConnectedAsync(this IMediaControl1 o) => o.GetAsync<bool>("Connected");
        public static Task<ObjectPath> GetPlayerAsync(this IMediaControl1 o) => o.GetAsync<ObjectPath>("Player");
    }

    [DBusInterface("org.bluez.Network1")]
    interface INetwork1 : IDBusObject
    {
        Task<string> ConnectAsync(string Uuid);
        Task DisconnectAsync();
        Task<T> GetAsync<T>(string prop);
        Task<Network1Properties> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }

    [Dictionary]
    class Network1Properties
    {
        private bool _Connected = default (bool);
        public bool Connected
        {
            get
            {
                return _Connected;
            }

            set
            {
                _Connected = (value);
            }
        }

        private string _Interface = default (string);
        public string Interface
        {
            get
            {
                return _Interface;
            }

            set
            {
                _Interface = (value);
            }
        }

        private string _UUID = default (string);
        public string UUID
        {
            get
            {
                return _UUID;
            }

            set
            {
                _UUID = (value);
            }
        }
    }

    static class Network1Extensions
    {
        public static Task<bool> GetConnectedAsync(this INetwork1 o) => o.GetAsync<bool>("Connected");
        public static Task<string> GetInterfaceAsync(this INetwork1 o) => o.GetAsync<string>("Interface");
        public static Task<string> GetUUIDAsync(this INetwork1 o) => o.GetAsync<string>("UUID");
    }
}
