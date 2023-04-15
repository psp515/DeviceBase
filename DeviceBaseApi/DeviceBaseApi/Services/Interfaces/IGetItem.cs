namespace DeviceBaseApi.Services.Interfaces;

public interface IGetItem<T>
{
    Task<T> GetItem(int id);
}

