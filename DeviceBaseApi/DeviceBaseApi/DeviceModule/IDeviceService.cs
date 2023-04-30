using DeviceBaseApi.DeviceTypeModule;
using DeviceBaseApi.Interfaces;

namespace DeviceBaseApi.DeviceModule;

public interface IDeviceService : IGetAllAsync<Device>, IUpdateAsync<Device>, ICreateAsync<Device>, IGetAsync<Device>
{
    Task<IEnumerable<Device>> GetUserItemsAsync(string id);
    Task<Connection> ConnectDevice(int deviceId, string userId);
    Task<Connection> DisconnectDevice(int deviceId, string userId);
    Task<bool> IsUserConnected(string guid, int id);
}
