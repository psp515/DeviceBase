using DeviceBaseApi.Models;

namespace DeviceBaseApi.Interfaces;

public interface IUpdateAsync<T> where T : BaseModel
{
    public DataContext db { get; }
    async Task<T> UpdateAsync(T item)
    {

        var newItem = db.GetDbSet<T>().Update(item).Entity;
        await db.SaveChangesAsync();

        return newItem;
    }
}
