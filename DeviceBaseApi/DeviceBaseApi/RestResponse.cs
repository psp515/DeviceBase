using System.Net;

namespace DeviceBaseApi;

public class RestResponse
{
    public RestResponse(string errorMessage = "")
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

    public RestResponse(string errorMessage = "", 
                        bool isSuccess = false, 
                        HttpStatusCode defaultStatus = HttpStatusCode.BadRequest,
                        object result = null)
    {
        ErrorMessage = errorMessage;
        IsSuccess = isSuccess;
        Result = result;
        StatusCode = defaultStatus;
    }

    public bool IsSuccess { get; set; }
    public Object Result { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string ErrorMessage { get; set; }
}
