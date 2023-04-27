using DeviceBaseApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DeviceBaseApi.Interfaces;

public interface IGetAsync<T> where T : BaseModel
{
    protected DataContext db { get; }

    async Task<ICollection<T>> GetAllAsync()
    {
        var itemCollection = await db.GetDbSet<T>().ToListAsync();
        return itemCollection;
    }

    async Task<T> GetAsync(int id)
    {
        var foundItem = await db.GetDbSet<T>().FindAsync(id);
        return foundItem;
    }
}
