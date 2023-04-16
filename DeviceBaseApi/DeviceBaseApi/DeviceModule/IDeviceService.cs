using DeviceBaseApi.Interfaces;

namespace DeviceBaseApi.DeviceModule;

public interface IDeviceService : IGetAsync<Device, int>, IUpdateAsync<Device>, ICreateAsync<Device>
{
    Task<ICollection<Device>> GetByUserIdAsync(string id);
    Task<bool> ConnectDevice(int deviceId, string userId);
    Task<bool> DisconnectDevice(int deviceId, string userId);
}
