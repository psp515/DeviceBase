namespace DeviceBaseApi.Services.Interfaces;

public interface IUpdateItem<T>
{
	Task<int> UpdateItem(Task item);
}

