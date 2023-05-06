using DeviceBaseApi.Models;

namespace DeviceBaseApi.Interfaces;

public interface IUpdateAsync<T> where T : BaseModel
{
    Task<T> UpdateAsync(T item);
}
