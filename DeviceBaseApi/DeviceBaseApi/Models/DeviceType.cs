
namespace DeviceBaseApi.Models;

public class DeviceType
{
    public int DeviceTypeId { get; set; }

    public string DefaultName { get; set; } = string.Empty;

    public int MaximalNumberOfUsers { get; set; }

    public ICollection<string> MqttEndpoints { get; set; } = new List<string>();

    public bool SoftDeleted { get; set; }

    public long EditionTics { get; set; }
    public long CreationTics { get; set; }
}