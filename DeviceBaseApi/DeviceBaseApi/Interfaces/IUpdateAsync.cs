using DeviceBaseApi.Models;

namespace DeviceBaseApi.Interfaces;

public interface IUpdateAsync<T> where T : BaseModel
{
    public DataContext db { get; }
    async Task<T> UpdateAsync(T item)
    {
        var foundItem = await db.GetDbSet<T>().FindAsync(item.Id);

        if (foundItem == null)
            return null;

        var newItem = db.GetDbSet<T>().Update(item).Entity;
        await db.SaveChangesAsync();

        return newItem;
    }
}
