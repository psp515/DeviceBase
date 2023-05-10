namespace DeviceBaseApi.DeviceModule.DTO;

public class DeviceCreateDTO
{
    public int DeviceTypeId { get; set; }
    public string MqttUrl { get; set; }
    public string SerialNumber { get; set; }
    public string Secret { get; set; }
    public DateTime Produced { get; set; }
}
