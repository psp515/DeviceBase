using DeviceBaseApi.Interfaces;

namespace DeviceBaseApi.DeviceModule;

public interface IDeviceService : IGetAsync<Device>, IUpdateAsync<Device>, ICreateAsync<Device>
{
    Task<ICollection<Device>> GetByUserIdAsync(string id);

    Task<bool> IsUserConnected(string userId, int deviceId);
    Task<bool> ConnectDevice(int deviceId, string userId);
    Task<bool> DisconnectDevice(int deviceId, string userId);
}
