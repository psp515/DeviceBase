namespace DeviceBaseApi;

public record ServiceResult(bool Success, string Error = "", object Value = null);