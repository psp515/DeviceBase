
using DeviceBaseApi.DeviceTypeModule;

namespace DeviceBaseApiTests.DeviceTypeModule;

public class TestDeviceTypeService : IDeviceTypeService
{

    List<DeviceType> DeviceTypes = new List<DeviceType>
    {
        new DeviceType
        {
            Id = 1,
            Created = DateTime.Now,
            Edited = DateTime.Now,
            DefaultName = "SP611",
            MaximalNumberOfUsers = 5,
            EndpointsJson = "[\"state\",\"mode\",\"ping\"]"
        },
        new DeviceType
        {
            Id = 2,
            Created = DateTime.Now,
            Edited = DateTime.Now,
            DefaultName = "SP611E",
            MaximalNumberOfUsers = 10,
            EndpointsJson = "[\"state\",\"mode\",\"ping\"]"
        }
    };

    private int id = 2;

    public async Task<DeviceType> CreateAsync(DeviceType item)
    {
        id++;
        item.Id = id;
        DeviceTypes.Add(item);
        return await Task.FromResult(item);
    }

    public async Task<IEnumerable<DeviceType>> GetAllAsync() => await Task.FromResult(DeviceTypes);

    public async Task<DeviceType?> GetAsync(int id)
    {
        var item = DeviceTypes.SingleOrDefault(x=>x.Id == id);

        if (item == null)
            return null;

        return await Task.FromResult(item);
    }

    public async Task<DeviceType?> UpdateAsync(DeviceType item)
    {
        var oldItem = DeviceTypes.SingleOrDefault(x => x.Id == item.Id);

        if (oldItem == null)
            return null;

        DeviceTypes.Remove(oldItem);
        DeviceTypes.Add(item);

        return await Task.FromResult(item);
    }
}
