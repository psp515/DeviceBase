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
    /* User is allowed to make request only if he is logged so we don't need to check if user exist beacuse he must. */

    /* Fixes: 
     * - problem when logged user is making request on other user id so that will be needed to check if token id == request id - get methods 
     * - could improve performace of update / connect / diconnect actions
     */

    public void Configure(WebApplication application)
    {
        /* Admin Policies */
        application.MapGet("/api/devices", GetDevices)
            .WithName("GetDevice")
            .Produces<RestResponse>(200)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.AdminPolicy);

        application.MapPost("/api/device", CreateDevice)
            .WithName("CreateDevice")
            .Accepts<CouponCreateDTO>("application/json")
            .Produces<RestResponse>(201)
            .Produces(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.AdminPolicy);

        /* Application User Policies */

        application.MapGet("/api/device/{guid:guid}/{id:int}", GetDevice)
            .WithName("GetDevices")
            .Produces<RestResponse>(200)
            .Produces(401)
            .RequireAuthorization();

        application.MapGet("/api/device/{guid:guid}", GetUserDevices)
            .WithName("GetUserDevices")
            .Produces<RestResponse>(200)
            .Produces(401)
            .RequireAuthorization();

        application.MapPut("/api/device/{guid:guid}", UpdateDevice)
            .WithName("UpdateDevice")
            .Accepts<DeviceUpdateDTO>("application/json")
            .Produces<RestResponse>(200)
            .Produces(400)
            .Produces(401)
            .RequireAuthorization();

        application.MapPatch("/api/device/{id:int}/connect/{guid:guid}", ConnectDevice)
            .WithName("ConnectDevice")
            .Produces<RestResponse>(200)
            .Produces<string>(400)
            .Produces(401)
            .RequireAuthorization();

        application.MapPatch("/api/device/{id:int}/diconnect/{guid:guid}", DisconnectDevice)
            .WithName("DisconnectDevice")
            .Produces<RestResponse>(200)
            .Produces<string>(400)
            .Produces(401)
            .RequireAuthorization();
    }

    private async Task<IResult> ConnectDevice(IDeviceService service, ILogger<Program> logger, string guid, int id)
    {
        var deviceExist = await service.ExistAsync(id);

        if (!deviceExist)
            return Results.BadRequest(new RestResponse("Device is not existing."));

        var userConnected = await service.IsUserConnected(guid, id);

        if (userConnected)
            return Results.BadRequest(new RestResponse("User is connected to this device."));

        // TODO: when device type add - can connect to device

        //var userConnected = await service.IsUserConnected(guid, id);

        //if (!userConnected)
        //    return Results.BadRequest(new RestResponse("User is connected to this device."));

        await service.ConnectDevice(id, guid);

        await service.SaveAsync();

        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, null));
    }

    private async Task<IResult> DisconnectDevice(IDeviceService service, ILogger<Program> logger, string guid, int id)
    {
        var userConnected = await service.IsUserConnected(guid, id);

        if (!userConnected)
            return Results.BadRequest(new RestResponse("User is connected to this device."));

        await service.DisconnectDevice(id, guid);

        await service.SaveAsync();

        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, null));
    }

    private async Task<IResult> GetDevice(IDeviceService service, ILogger<Program> logger, int id)
    {
        var deviceExist = await service.ExistAsync(id);

        if (!deviceExist)
            return Results.BadRequest(new RestResponse("Device is not existing."));

        var result = await service.GetAsync(id);
        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, result));

    }
    private async Task<IResult> GetDevices(IDeviceService service, ILogger<Program> logger)
    {
        var result = await service.GetAllAsync();
        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, result));
    }
    private async Task<IResult> GetUserDevices(IDeviceService service, ILogger<Program> logger, string guid)
    {
        var result = await service.GetByUserIdAsync(guid);
        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, result));
    }
    

    private async Task<IResult> UpdateDevice(IDeviceService service, 
        IValidator<DeviceUpdateDTO> validation, 
        ILogger<Program> logger, 
        IMapper mapper,
        string guid,
        [FromBody] DeviceUpdateDTO request)
    {

        var deviceExist = await service.ExistAsync(request.DeviceId);

        if (!deviceExist)
            return Results.BadRequest(new RestResponse("Device is not existing."));

        var userConnected = await service.IsUserConnected(guid, request.DeviceId);

        if (!userConnected)
            return Results.BadRequest(new RestResponse("User is not authorised to update this device."));

        var validationResult = await validation.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.BadRequest(new RestResponse(validationResult.Errors.FirstOrDefault().ErrorMessage));


        var device = mapper.Map<Device>(request);

        await service.UpdateAsync(device);
        await service.SaveAsync();

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

        //TODO: set base fields 
        var device = mapper.Map<Device>(request);
        await service.CreateAsync(device);
        await service.SaveAsync();

        return Results.Created($"/api/device/{device.DeviceId}", new RestResponse(HttpStatusCode.Created, true, null));
    }

}
