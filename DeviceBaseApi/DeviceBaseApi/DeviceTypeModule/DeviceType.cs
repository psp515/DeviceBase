
using System.ComponentModel.DataAnnotations;

namespace DeviceBaseApi.DeviceTypeModule;

public class DeviceType : BaseModel
{
    public string DefaultName { get; set; } = string.Empty;

    public int MaximalNumberOfUsers { get; set; }

    public ICollection<string> MqttEndpoints { get; set; } = new List<string>();
}