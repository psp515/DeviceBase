using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace DeviceBaseApi.DeviceModule;

public class DeviceService : BaseService, IDeviceService
{
    public DeviceService(DataContext db, IMapper mapper) : base(db, mapper)
    {

    }

    public Task<bool> ConnectDevice(int deviceId, string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DisconnectDevice(int deviceId, string userId)
    {
        throw new NotImplementedException();
    }


    public async Task<Device> CreateAsync(Device item)
    {
        var newItem = await db.Devices.AddAsync(item);
        return newItem.Entity;
    }
    public async Task<ICollection<Device>> GetAllAsync()
    {
        return await db.Devices.ToListAsync();
    }
    public async Task<Device> GetAsync(int id)
    {
        return await db.Devices.FirstAsync(item => item.DeviceId == id);
    }
    public async Task<ICollection<Device>> GetByUserIdAsync(string id)
    {
        var user = await db.AppUsers.FirstAsync(x => x.Id == id);
        return user.Devices;
    }
    public async Task<bool> SaveAsync()
    {
        try 
        {
            await db.SaveChangesAsync();
            return true;
        }
        catch(Exception ex) 
        {
            return false;
        }
    }
    public async Task<Device> UpdateAsync(Device item)
    {
        var newItem = db.Devices.Update(item);
        return newItem.Entity;
    }
}
