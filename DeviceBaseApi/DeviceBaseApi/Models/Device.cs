using DeviceBaseApi.AuthModule;
using System.ComponentModel.DataAnnotations;

namespace DeviceBaseApi.Models;

public class Device
{
    [Key]
    public int DeviceId { get; set; }

    public string Url { get; set; }
    public bool IsWorking { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();

    public DeviceType? DeviceType { get; set; }

    public bool SoftDeleted { get; set; }

    public DateTime Edited { get; set; }
    public long Created{ get; set; }
}

