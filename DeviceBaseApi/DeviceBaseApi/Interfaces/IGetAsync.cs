using DeviceBaseApi.Models;

namespace DeviceBaseApi.Interfaces;

public interface IGetAsync<T> where T : BaseModel
{
    Task<T> GetAsync(int id);
}
