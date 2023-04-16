using AutoMapper;
using Azure.Core;
using DeviceBaseApi.Coupons.DTO;
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
        application.MapGet("/api/devices", GetDevices)
            .WithName("GetDevice")
            .Produces<RestResponse>(200)
            .Produces(401)
            .RequireAuthorization();

        application.MapGet("/api/device/{id:int}", GetDevice)
            .WithName("GetDevices")
            .Produces<RestResponse>(200)
            .Produces(401)
            .RequireAuthorization();

        application.MapGet("/api/device/{id:string}", GetUserDevices)
            .WithName("GetUserDevices")
            .Produces<RestResponse>(200)
            .Produces(401)
            .RequireAuthorization();

        application.MapPost("/api/device", CreateDevice)
            .WithName("CreateDevice")
            .Accepts<CouponCreateDTO>("application/json")
            .Produces<RestResponse>(201)
            .Produces(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.AdminPolicy);

        application.MapPut("/api/device/{id:int}", UpdateDevice)
            .WithName("UpdateDevice")
            .Accepts<CouponUpdateDTO>("application/json")
            .Produces<RestResponse>(200)
            .Produces(400)
            .Produces(401)
            .RequireAuthorization();

    }

    private async Task<IResult> ConnectToDevice(IDeviceService service, ILogger<Program> logger, string id)
    {
        // result = await de.GetAsync(id);
        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, null));
    }

    private async Task<IResult> DisconnectFromDevice(IDeviceService service, ILogger<Program> logger, string id)
    {
        //var result = await de.GetAsync(id);
        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, null));
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
        //TODO check if device exists
        //TODO chekc if user has device

        var validationResult = await validation.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.BadRequest(new RestResponse(validationResult.Errors.FirstOrDefault().ErrorMessage));

        var device = mapper.Map<Device>(request);
        var result = await service.UpdateAsync(device);

        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, null));
    }

    private async Task<IResult> CreateDevice(IDeviceService service, 
                                IValidator<DeviceCreateDTO> validation,
                                IMapper mapper,
                                ILogger<Program> logger,
                                [FromBody] DeviceCreateDTO request)
    {

        var validationResult = await validation.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.BadRequest(new RestResponse(validationResult.Errors.FirstOrDefault().ErrorMessage));

        var device = mapper.Map<Device>(request);
        var result = await service.CreateAsync(device);

        return Results.Created($"/api/device/{device.DeviceId}", new RestResponse(HttpStatusCode.Created, true, null));
    }

}
