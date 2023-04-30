using DeviceBaseApi.Models;

namespace DeviceBaseApi.Interfaces;

public interface IUpdateAsync<T, TId> : ISave, IExistAsync<TId>
{
    Task<T> UpdateAsync(T item);
}
