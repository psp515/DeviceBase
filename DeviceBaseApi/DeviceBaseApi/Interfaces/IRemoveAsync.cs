using DeviceBaseApi.Models;

namespace DeviceBaseApi.Interfaces;

public interface IRemoveAsync<T>
{
    Task<bool> RemoveAsync(T coupon);
}
