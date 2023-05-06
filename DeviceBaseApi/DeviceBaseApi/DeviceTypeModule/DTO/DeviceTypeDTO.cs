namespace DeviceBaseApi.DeviceTypeModule.DTO;

public record DeviceTypeDTO(
    int Id, 
    DateTime Edited,
    DateTime Created, 
    string DefaultName, 
    int MaximalNumberOfUsers, 
    string EndpointsJson);
