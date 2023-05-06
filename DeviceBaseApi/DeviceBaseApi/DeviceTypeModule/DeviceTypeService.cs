using Microsoft.EntityFrameworkCore;

namespace DeviceBaseApi.DeviceTypeModule;

public class DeviceTypeService : BaseService, IDeviceTypeService
{
    public DeviceTypeService(DataContext db) : base(db) { }

    public async Task<DeviceType> CreateAsync(DeviceType item)
    {
        var newItem = await db.DeviceTypes.AddAsync(item);
        await db.SaveChangesAsync();
        return newItem.Entity;
    }

    public async Task<DeviceType> GetAsync(int id)
    {
        var foundItem = await db.DeviceTypes.FindAsync(id);
        return foundItem;
    }

    public async Task<IEnumerable<DeviceType>> GetAllAsync()
    {
        var itemCollection = await db.DeviceTypes.ToListAsync();
        return itemCollection;
    }

    public async Task<DeviceType> UpdateAsync(DeviceType item)
    {
        var newItem = db.DeviceTypes.Update(item).Entity;
        await db.SaveChangesAsync();
        return newItem;
    }
}

