using DeviceBaseApi.Interfaces;
using DeviceBaseApi.Models;

namespace DeviceBaseApi.DeviceModule;

public interface IDeviceService : IGetAllAsync<Device>, IUpdateAsync<Device>, ICreateAsync<Device>, IGetAsync<Device>
{
    Task<IEnumerable<Device>> GetUserItemsAsync(string id);

    Task<ServiceResult> DeviceConnectionsPolicy(int deviceId, string userId, bool newPolicy);
    Task<ServiceResult> ConnectOwner(int deviceId, string userId, string secret);
    Task<ServiceResult> DisconnectOwner(int deviceId, string userId);
    Task<ServiceResult> ConnectDevice(int deviceId, string userId);
    Task<ServiceResult> DisconnectDevice(int deviceId, string userId);
    Task<bool> IsUserConnected(string guid, int id);
}
