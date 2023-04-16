using DeviceBaseApi.AuthModule;
using System.ComponentModel.DataAnnotations;

namespace DeviceBaseApi.DeviceModule;

public class Device
{
    [Key]
    public int DeviceId { get; set; }

    public string DeviceName { get; set; }
    public string DevicePlacing { get; set; }
    public string Description { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
    //public DeviceType DeviceType { get; set; }

    public string MqttUrl { get; set; }
    public string SerialNumber { get; set; }

    public DateTime Produced { get; set; }
    public DateTime Edited { get; set; }
    public DateTime Created { get; set; }
}

