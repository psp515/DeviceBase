using System;
using DeviceBaseApi.DeviceModule;
using DeviceBaseApi.DeviceModule.DTO;
using DeviceBaseApi.DeviceTypeModule;

namespace DeviceBaseApi.DeviceModule;

public static class DeviceExtensions
{
    public static Device CreateDevice(this DeviceCreateDTO dto, DeviceType deviceType)
    {
        return new Device
        {
            Edited = DateTime.Now,
            Created = DateTime.Now,

            DeviceName = deviceType.DefaultName,
            Description = "",
            DevicePlacing = "",

            DeviceType = deviceType,
            MqttUrl = dto.MqttUrl,
            Produced = dto.Produced,
            SerialNumber = dto.SerialNumber
        };
    }

    public static Device UpdateDevice(this DeviceUpdateDTO dto, int id)
    {
        return new Device
        {
            Id = id,
            Edited = DateTime.Now,

            DeviceName = dto.DeviceName,
            Description = dto.Description,
            DevicePlacing = dto.DevicePlacing
        };
    }
}

