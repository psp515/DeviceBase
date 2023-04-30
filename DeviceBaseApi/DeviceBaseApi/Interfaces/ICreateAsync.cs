namespace DeviceBaseApi.Interfaces;

public interface ICreateAsync<T> where T : BaseModel
{
    public DataContext db { get; }

    async Task<T> CreateAsync(T item)
    {
        var newItem = await db.GetDbSet<T>().AddAsync(item);
        await db.SaveChangesAsync();
        return newItem.Entity;
    }
}
