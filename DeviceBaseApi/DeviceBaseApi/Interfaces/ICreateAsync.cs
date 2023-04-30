using DeviceBaseApi.Models;

namespace DeviceBaseApi.Interfaces;

public interface ICreateAsync<T> : ISave
{
    Task<T> CreateAsync(T item);
}
