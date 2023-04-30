namespace DeviceBaseApi;

public record ServiceResult(bool Success, string Error = "", object Value = null);

public record ServiceResult<T>(bool Success, string Error = "", T Value = null) where T : class;