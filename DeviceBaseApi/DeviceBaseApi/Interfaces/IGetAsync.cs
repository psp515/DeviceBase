using DeviceBaseApi.Models;

namespace DeviceBaseApi.Interfaces;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"> Type of object to get. </typeparam>
/// <typeparam name="T1"> Type of id field. </typeparam>
public interface IGetAsync<T, T1>
{
    Task<ICollection<T>> GetAllAsync();
    Task<T> GetAsync(T1 id);
}
