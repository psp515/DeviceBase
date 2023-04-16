using DeviceBaseApi.Models;

namespace DeviceBaseApi.Interfaces;

public interface IUpdateAsync<T> : ISave
{
    Task<T> UpdateAsync(T item);
}
