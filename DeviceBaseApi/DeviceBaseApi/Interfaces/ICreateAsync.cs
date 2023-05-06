using DeviceBaseApi.Models;

namespace DeviceBaseApi.Interfaces;

public interface ICreateAsync<T> where T : BaseModel
{
    Task<T> CreateAsync(T item);
}
