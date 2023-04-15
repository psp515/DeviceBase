namespace DeviceBaseApi.Services.Interfaces;

public interface IAddItem<T>
{
    Task<int> AddItem(T item);
}

