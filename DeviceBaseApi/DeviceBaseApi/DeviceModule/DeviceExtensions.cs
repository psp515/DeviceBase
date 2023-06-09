﻿using DeviceBaseApi.DeviceModule.DTO;
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
            NewConnectionsPermitted = true,

            DeviceSecret = dto.Secret,
            DeviceType = deviceType,
            MqttUrl = dto.MqttUrl,
            Produced = dto.Produced,
            SerialNumber = dto.SerialNumber
        };
    }

    public static void UpdateDevice(this Device device, DeviceUpdateDTO dto)
    {
        device.Edited = DateTime.Now;
        device.DeviceName = dto.DeviceName;
        device.Description = dto.Description;
        device.DevicePlacing = dto.DevicePlacing;
    }

    public static DeviceDTO ToDeviceDTO(this Device device) 
    {
        return new DeviceDTO(
            device.Id,
            device.Edited,
            device.Created,
            device.DeviceName,
            device.DevicePlacing,
            device.DeviceTypeId,
            device.NewConnectionsPermitted,
            device.OwnerId,
            device.MqttUrl,
            device.SerialNumber,
            device.Produced);
    }
}

