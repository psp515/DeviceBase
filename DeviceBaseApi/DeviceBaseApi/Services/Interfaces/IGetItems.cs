namespace DeviceBaseApi.Services.Interfaces;

public interface IGetItems<T>
{
    Task<IEnumerable<T>> GetItems();
}

