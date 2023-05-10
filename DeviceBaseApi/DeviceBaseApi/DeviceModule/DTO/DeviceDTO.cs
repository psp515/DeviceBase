namespace DeviceBaseApi.DeviceModule.DTO;

public record DeviceDTO(
    int Id,
    DateTime Edited,
    DateTime Created,
    string DeviceName,
    string DevicePlacing,
    int DeviceTypeId,
    bool NewConnectionsPermitted,
    string ownerId,
    string MqttUrl,
    string SerialNumber,
    DateTime Produced);
