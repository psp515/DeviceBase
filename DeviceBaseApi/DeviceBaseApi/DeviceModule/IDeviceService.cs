using DeviceBaseApi.DeviceTypeModule;
using DeviceBaseApi.Interfaces;

namespace DeviceBaseApi.DeviceModule;

public interface IDeviceService : IGetAsync<Device>, IUpdateAsync<Device>, ICreateAsync<Device>
{
    Task<ICollection<Device>> GetUserItems(string id);
    Task<Connection> ConnectDevice(int deviceId, string userId);
    Task<Connection> DisconnectDevice(int deviceId, string userId);
    Task<bool> IsUserConnected(string guid, int id);
}
