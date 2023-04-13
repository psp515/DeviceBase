
namespace DeviceBaseApi.Models;

public class Device
{
    public int DeviceId { get; set; }

    public string Url { get; set; }
    public bool IsWorking { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();

    public DeviceType? DeviceType { get; set; }

    public bool SoftDeleted { get; set; }

    public long EditionTics { get; set; }
    public long CreationTics { get; set; }
}

