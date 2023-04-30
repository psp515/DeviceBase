﻿using System.Net;

namespace DeviceBaseApi.Models;

public class RestResponse
{
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
