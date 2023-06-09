﻿using DeviceBaseApi.DeviceModule.DTO;
using DeviceBaseApi.DeviceTypeModule;
using DeviceBaseApi.Interfaces;
using DeviceBaseApi.Models;
using DeviceBaseApi.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DeviceBaseApi.DeviceModule;

public class DeviceEndpoints : IEndpoints
{
    public void Configure(WebApplication application)
    {
        /* Admin Policies */

        application.MapGet("/api/devices", GetAllAsync)
            .WithName("Get Devices")
            .Produces<RestResponse>(200)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.AdminPolicy);

        application.MapGet("/api/devices/{id:int}", GetItemAsync)
            .WithName("Get Device")
            .Produces<RestResponse>(200)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.AdminPolicy);

        application.MapPost("/api/devices", CreateItem)
            .WithName("Create Device")
            .Accepts<DeviceCreateDTO>("application/json")
            .Produces<RestResponse>(201)
            .Produces(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.AdminPolicy);

        /* Application User Policies */

        application.MapGet("/api/devices/user", GetUserItemsAsync)
            .WithName("Get User Devices")
            .Produces<RestResponse>(200)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);

        application.MapGet("/api/devices/{id:int}/users", GetDeviceUsers)
            .WithName("Get Users Connected to Device")
            .Produces<RestResponse>(200)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);

        application.MapPut("/api/devices/{id:int}", UpdateItem)
            .WithName("Update Device")
            .Accepts<DeviceUpdateDTO>("application/json")
            .Produces(204)
            .Produces<RestResponse>(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);

        application.MapPatch("/api/devices/{id:int}/connect", ConnectDevice)
            .WithName("Connect Device")
            .Produces(204)
            .Produces<RestResponse>(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);

        application.MapPatch("/api/devices/{id:int}/disconnect", DisconnectDevice)
            .WithName("Disconnect Device")
            .Produces(204)
            .Produces<RestResponse>(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);

        application.MapPatch("/api/devices/{id:int}/connectowner", ConnectOwner)
            .WithName("Connect Owner to Device")
            .Produces(204)
            .Produces<RestResponse>(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);

        application.MapPatch("/api/devices/{id:int}/disconnectowner", DisconnectOwner)
            .WithName("Disconnect Owner from Device")
            .Produces(204)
            .Produces<RestResponse>(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);

        application.MapPatch("/api/devices/{id:int}/ownerdisconnectuser", OwnerDisconnectsUser)
            .WithName("Disconnect User from Device By Owner")
            .Accepts<DisconnectUserDTO>("application/json")
            .Produces(204)
            .Produces<RestResponse>(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);

        application.MapPatch("/api/devices/{id:int}", ChangeDevicePolicy)
            .WithName("Change Device Connection Policy")
            .Produces(204)
            .Produces<RestResponse>(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);
    }

    [Authorize]
    private async Task<IResult> GetItemAsync(IDeviceService service, int id)
    {
        var result = await service.GetAsync(id);

        if (result == null)
            return Results.BadRequest(new RestResponse("Device is not existing."));

        var dto = result.ToDeviceDTO();

        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, dto));

    }

    [Authorize]
    private async Task<IResult> GetAllAsync(IDeviceService service)
    {
        var result = await service.GetAllAsync();

        var dto = result.Select(x => x.ToDeviceDTO());

        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, dto));
    }

    [Authorize]
    private async Task<IResult> GetUserItemsAsync(IDeviceService service,
                                                  IConfiguration configuration,
                                                  [FromHeader(Name = "Authorization")] string bearerToken)
    {
        var guid = bearerToken.GetValueFromToken(configuration.GetValue<string>("ApiSettings:Secret"));

        if (guid == null)
            return Results.BadRequest(new RestResponse("Invalid token."));

        var result = await service.GetUserItemsAsync(guid);

        if (result == null)
            return Results.BadRequest(new RestResponse("User not found."));

        var dto = result.Select(x => x.ToDeviceDTO());

        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, dto));
    }

    [Authorize]
    private async Task<IResult> UpdateItem(IDeviceService service,
                                           IValidator<DeviceUpdateDTO> validation,
                                           IConfiguration configuration,
                                           [FromBody] DeviceUpdateDTO request,
                                           [FromHeader(Name = "Authorization")] string bearerToken,
                                           int id)
    {

        var guid = bearerToken.GetValueFromToken(configuration.GetValue<string>("ApiSettings:Secret"));

        if (guid == null)
            return Results.BadRequest(new RestResponse("Invalid token."));

        var userConnected = await service.IsUserConnected(guid, id);

        if (!userConnected)
            return Results.BadRequest(new RestResponse("User is not authorized to update this device."));

        var device = await service.GetAsync(id);

        if (device == null)
            return Results.BadRequest(new RestResponse("Device Type not found"));

        var validationResult = await validation.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.BadRequest(new RestResponse(validationResult.Errors.FirstOrDefault().ErrorMessage));

        device.UpdateDevice(request);

        var updatedDevice = await service.UpdateAsync(device);

        if (updatedDevice == null)
            return Results.BadRequest(new RestResponse("Device not updated. Item not found."));

        return Results.NoContent();
    }

    [Authorize]
    private async Task<IResult> CreateItem(IDeviceService deviceService,
                                           IDeviceTypeService deviceTypeService,
                                           IValidator<DeviceCreateDTO> validation,
                                           [FromBody] DeviceCreateDTO request)
    {
        var validationResult = await validation.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.BadRequest(new RestResponse(validationResult.Errors.FirstOrDefault().ErrorMessage));

        var deviceType = await deviceTypeService.GetAsync(request.DeviceTypeId);

        if (deviceType == null)
            return Results.BadRequest(new RestResponse("Device type not found."));

        var device = request.CreateDevice(deviceType);
        var createdDevice = await deviceService.CreateAsync(device);

        return Results.Created($"/api/device/{createdDevice.Id}", new RestResponse(HttpStatusCode.Created, true, null));
    }

    [Authorize]
    private async Task<IResult> ConnectDevice(IDeviceService service,
                                              IConfiguration configuration,
                                              [FromHeader(Name = "Authorization")] string bearerToken,
                                              int id)
    {

        var guid = bearerToken.GetValueFromToken(configuration.GetValue<string>("ApiSettings:Secret"));

        if (guid == null)
            return Results.BadRequest(new RestResponse("Invalid token."));

        var connection = await service.ConnectDevice(id, guid);

        if (!connection.Success)
            return Results.BadRequest(new RestResponse(connection.Error));

        return Results.NoContent();
    }

    [Authorize]
    private async Task<IResult> ConnectOwner(IDeviceService service,
                                             IConfiguration configuration,
                                             [FromHeader(Name = "Authorization")] string bearerToken,
                                             [FromBody] OwnerConnectionDTO dto,
                                             int id)
    {

        var guid = bearerToken.GetValueFromToken(configuration.GetValue<string>("ApiSettings:Secret"));

        if (guid == null)
            return Results.BadRequest(new RestResponse("Invalid token."));

        var connection = await service.ConnectOwner(id, guid, dto.DeviceSecret);

        if (!connection.Success)
            return Results.BadRequest(new RestResponse(connection.Error));

        return Results.NoContent();
    }

    [Authorize]
    private async Task<IResult> DisconnectOwner(IDeviceService service,
                                             IConfiguration configuration,
                                             [FromHeader(Name = "Authorization")] string bearerToken,
                                             int id)
    {

        var guid = bearerToken.GetValueFromToken(configuration.GetValue<string>("ApiSettings:Secret"));

        if (guid == null)
            return Results.BadRequest(new RestResponse("Invalid token."));

        var connection = await service.DisconnectOwner(id, guid);

        if (!connection.Success)
            return Results.BadRequest(new RestResponse(connection.Error));

        return Results.NoContent();
    }

    [Authorize]
    private async Task<IResult> DisconnectDevice(IDeviceService service,
                                                 IConfiguration configuration,
                                                 [FromHeader(Name = "Authorization")] string bearerToken,
                                                 int id)
    {

        var guid = bearerToken.GetValueFromToken(configuration.GetValue<string>("ApiSettings:Secret"));

        if (guid == null)
            return Results.BadRequest(new RestResponse("Invalid token."));

        var connection = await service.DisconnectDevice(id, guid);

        if (!connection.Success)
            return Results.BadRequest(new RestResponse(connection.Error));

        return Results.NoContent();
    }

    [Authorize]
    private async Task<IResult> ChangeDevicePolicy(IDeviceService service,
                                                 IConfiguration configuration,
                                                 [FromHeader(Name = "Authorization")] string bearerToken,
                                                 [FromBody] ConnectionPolicyDTO dto,
                                                 int id)
    {

        var guid = bearerToken.GetValueFromToken(configuration.GetValue<string>("ApiSettings:Secret"));

        if (guid == null)
            return Results.BadRequest(new RestResponse("Invalid token."));

        var connection = await service.DeviceConnectionsPolicy(id, guid, dto.ConnectionEnabled);

        if (!connection.Success)
            return Results.BadRequest(new RestResponse(connection.Error));

        return Results.NoContent();
    }

    [Authorize]
    private async Task<IResult> GetDeviceUsers(IDeviceService service,
                                             IConfiguration configuration,
                                             [FromHeader(Name = "Authorization")] string bearerToken,
                                             int id)
    {

        var guid = bearerToken.GetValueFromToken(configuration.GetValue<string>("ApiSettings:Secret"));

        if (guid == null)
            return Results.BadRequest(new RestResponse("Invalid token."));

        var isOwner = await service.IsDeviceOwner(guid, id);

        if (!isOwner)
            return Results.BadRequest(new RestResponse("User is not owner or device not found."));

        var users = await service.GetDeviceUsers(id);

        var dto = users.Select(x => x.ToConnectedUserDTO());

        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, dto));
    }

    [Authorize]
    private async Task<IResult> OwnerDisconnectsUser(IDeviceService service,
                                             IConfiguration configuration,
                                             [FromHeader(Name = "Authorization")] string bearerToken,
                                             [FromBody] DisconnectUserDTO dto,
                                             int id)
    {

        var guid = bearerToken.GetValueFromToken(configuration.GetValue<string>("ApiSettings:Secret"));

        if (guid == null)
            return Results.BadRequest(new RestResponse("Invalid token."));

        var isOwner = await service.IsDeviceOwner(guid, id);

        if (!isOwner)
            return Results.BadRequest(new RestResponse("User is not owner or device not found."));

        var disconnected = await service.DisconnectUser(dto.UserId, id);

        if (!disconnected)
            return Results.BadRequest(new RestResponse("Something went wrong."));

        return Results.NoContent();
    }

}
