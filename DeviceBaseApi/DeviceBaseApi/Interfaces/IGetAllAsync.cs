using DeviceBaseApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DeviceBaseApi.Interfaces;

public interface IGetAllAsync<T> where T : BaseModel
{
    Task<IEnumerable<T>> GetAllAsync();
}
