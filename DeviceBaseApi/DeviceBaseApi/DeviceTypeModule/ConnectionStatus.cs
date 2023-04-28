using System;
namespace DeviceBaseApi.DeviceTypeModule;

public record Connection(bool Success, string Error = "");