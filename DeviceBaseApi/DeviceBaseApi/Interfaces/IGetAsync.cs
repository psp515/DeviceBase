using Microsoft.EntityFrameworkCore;

namespace DeviceBaseApi.Interfaces;

public interface IGetAsync<T> where T : BaseModel
{
    protected DataContext db { get; }

    async Task<T> GetAsync(int id)
    {
        var foundItem = await db.GetDbSet<T>().FindAsync(id);
        return foundItem;
    }
}
