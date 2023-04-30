using Microsoft.EntityFrameworkCore;

namespace DeviceBaseApi.Interfaces;

public interface IGetAllAsync<T> where T : BaseModel
{
    protected DataContext db { get; }

    async Task<IEnumerable<T>> GetAllAsync()
    {
        var itemCollection = await db.GetDbSet<T>().ToListAsync();
        return itemCollection;
    }
}
