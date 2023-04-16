using AutoMapper;
using DeviceBaseApi.DeviceModule.DTO;
using DeviceBaseApi.Interfaces;
using DeviceBaseApi.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DeviceBaseApi.DeviceModule;

public class DeviceEndpoints : IEndpoint
{
    public void Configure(WebApplication application)
    {
        throw new NotImplementedException();
    }

    private async Task<IResult> ConnectToDevice(IDeviceService service, ILogger<Program> logger, string id)
    {
        var result = await de.GetAsync(id);
        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, result));
    }

    private async Task<IResult> DisconnectFromDevice(IDeviceService service, ILogger<Program> logger, string id)
    {
        var result = await de.GetAsync(id);
        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, result));
    }

    private async Task<IResult> GetDevice(IDeviceService service, ILogger<Program> logger, int id)
    {
        try 
        {
            var result = await service.GetAsync(id);
            return Results.Ok(new RestResponse(HttpStatusCode.OK, true, result));
        }
        catch (ArgumentNullException ex) 
        {
            return Results.BadRequest(new RestResponse("Invalid device id."));
        }
        catch
        {
            return Results.BadRequest("Unexpected error.");
        }
    }

    private async Task<IResult> GetDevices(IDeviceService service, ILogger<Program> logger)
    {
        var result = await service.GetAllAsync();
        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, result));
    }

    private async Task<IResult> GetUserDevices(IDeviceService service, ILogger<Program> logger, string id)
    {
        var result = await service.GetByUserIdAsync(id);
        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, result));
    }
    
    private async Task<IResult> UpdateDevice(IDeviceService service, 
        IValidator<DeviceUpdateDTO> validation, 
        ILogger<Program> logger, 
        IMapper mapper,
        string id,
        [FromBody] DeviceUpdateDTO request)
    {

      


        var validationResult = await validation.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.BadRequest(new RestResponse(validationResult.Errors.FirstOrDefault().ErrorMessage));

        var device = mapper.Map<Device>(request);
        var result = await service.UpdateAsync(device);

        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, result));
    }

    // ADMIN ONLY
    private async Task<IResult> CreateDevice(IDeviceService service, IValidator<DeviceUpdateDTO> validation, ILogger<Program> logger, int id)
    {
        var result = await de.GetAsync(id);
        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, result));
    }

}
