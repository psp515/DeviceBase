namespace DeviceBaseApi.DeviceModule.DTO;

public class DeviceCreateDTO
{
    public string MqttUrl { get; set; }
    public string SerialNumber { get; set; }
    public DateTime Produced { get; set; }
}
