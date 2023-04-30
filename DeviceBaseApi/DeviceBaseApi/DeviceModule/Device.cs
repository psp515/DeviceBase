using DeviceBaseApi.DeviceTypeModule;
using DeviceBaseApi.Models;

namespace DeviceBaseApi.DeviceModule;

public class Device : BaseModel
{
    public string DeviceName { get; set; }
    public string DevicePlacing { get; set; }
    public string Description { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();

    public int DeviceTypeId { get; set; }
    public DeviceType DeviceType { get; set; }

    public string MqttUrl { get; set; }
    public string SerialNumber { get; set; }

    public DateTime Produced { get; set; }
}

