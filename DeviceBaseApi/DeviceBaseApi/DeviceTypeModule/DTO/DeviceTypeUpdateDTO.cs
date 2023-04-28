using System;
using DeviceBaseApi.DeviceModule;

namespace DeviceBaseApi.DeviceTypeModule.DTO;

public record DeviceTypeUpdateDTO(string DefaultName,
    int MaximalNumberOfUsers,
    ICollection<Device> Devices);
