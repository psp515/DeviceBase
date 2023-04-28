using System;
using DeviceBaseApi.DeviceModule;

namespace DeviceBaseApi.DeviceTypeModule.DTO;

public record DeviceTypeCreateDTO(string DefaultName,
    int MaximalNumberOfUsers,
    string EndpointsJson,
    ICollection<Device> Devices);
