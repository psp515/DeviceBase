namespace DeviceBaseApi.Interfaces;

public interface IExistAsync<T>
{
    Task<bool> ExistAsync(T id);
}
