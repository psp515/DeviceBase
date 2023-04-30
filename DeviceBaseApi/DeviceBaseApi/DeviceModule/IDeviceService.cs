using DeviceBaseApi.Interfaces;

namespace DeviceBaseApi.DeviceModule;

public interface IDeviceService : IGetAllAsync<Device>, IUpdateAsync<Device>, ICreateAsync<Device>, IGetAsync<Device>
{
    Task<IEnumerable<Device>> GetUserItemsAsync(string id);
    Task<ServiceResult> ConnectDevice(int deviceId, string userId);
    Task<ServiceResult> DisconnectDevice(int deviceId, string userId);
    Task<bool> IsUserConnected(string guid, int id);
}
