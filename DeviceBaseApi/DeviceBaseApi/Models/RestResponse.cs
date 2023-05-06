using System.Net;
using System.Text.Json.Serialization;

namespace DeviceBaseApi.Models;

public class RestResponse
{
    [JsonConstructor]
    public RestResponse(HttpStatusCode StatusCode = HttpStatusCode.BadRequest,
                             bool IsSuccess = false,
                             object Result = null,
                             string ErrorMessage = "")
    {
        this.ErrorMessage = ErrorMessage;
        this.IsSuccess = IsSuccess;
        this.Result = Result;
        this.StatusCode = StatusCode;
    }

    public RestResponse(string errorMessage)
    {
        ErrorMessage = errorMessage;
        IsSuccess = false;
        Result = null;
        StatusCode = HttpStatusCode.BadRequest;
    }

    public RestResponse(HttpStatusCode defaultStatus = HttpStatusCode.OK,
                        bool isSuccess = true,
                        object result = null)
    {
        ErrorMessage = "";
        IsSuccess = isSuccess;
        Result = result;
        StatusCode = defaultStatus;
    }

    public RestResponse(string errorMessage,
                        bool isSuccess,
                        HttpStatusCode defaultStatus,
                        object result)
    {
        ErrorMessage = errorMessage;
        IsSuccess = isSuccess;
        Result = result;
        StatusCode = defaultStatus;
    }

    public bool IsSuccess { get; set; }
    public object Result { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string ErrorMessage { get; set; }
}
