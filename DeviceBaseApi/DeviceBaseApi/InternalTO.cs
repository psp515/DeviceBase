using DeviceBaseApi.AuthModule.DTO;

namespace DeviceBaseApi;

public class InternalTO<T> where T : class, new()
{
    public InternalTO(string error)
    {
        HasError = true;
        Error = error;
        Value = default(T);
    }

    public InternalTO(T value)
    {
        HasError = false;
        Error = null;
        Value = value;
    }

    public bool HasError { get; set; }
    public string Error { get; set; }

    public T Value { get; set; }
}