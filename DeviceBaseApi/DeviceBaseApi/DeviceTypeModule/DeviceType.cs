using DeviceBaseApi.DeviceModule;

namespace DeviceBaseApi.DeviceTypeModule;

public class DeviceType : BaseModel
{
    public string DefaultName { get; set; }

    public int MaximalNumberOfUsers { get; set; }

    public string EndpointsJson { get; set; }

    public ICollection<Device> Devices { get; set; } = new List<Device>();
}