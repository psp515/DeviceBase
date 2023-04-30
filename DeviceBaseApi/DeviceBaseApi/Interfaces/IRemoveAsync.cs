namespace DeviceBaseApi.Interfaces;

public interface IRemoveAsync<T> where T : BaseModel
{
    protected DataContext db { get; }
    async Task<bool> RemoveAsync(T item)
    {
        var foundItem = await db.GetDbSet<T>().FindAsync(item.Id);

        if (foundItem == null)
            return false;

        db.GetDbSet<T>().Remove(foundItem);
        await db.SaveChangesAsync();

        return true;
    }
}
