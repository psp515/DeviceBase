﻿using System;
using System.Net;
using AutoMapper;
using DeviceBaseApi.DeviceModule;
using DeviceBaseApi.DeviceModule.DTO;
using DeviceBaseApi.DeviceTypeModule.DTO;
using DeviceBaseApi.DeviceTypeModule.Validation;
using DeviceBaseApi.Interfaces;
using DeviceBaseApi.Models;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace DeviceBaseApi.DeviceTypeModule;

public class DeviceTypeEndpoints : IEndpoint
{
    public void Configure(WebApplication application)
    {
        /* Admin Policies */
        application.MapGet("/api/devicetypes", GetAllItemsAsync)
            .WithName("Get Device Types")
            .Produces<RestResponse>(200)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.AdminPolicy);

        application.MapPost("/api/devicetypes", CreateItemAsync)
            .WithName("Create Device Type")
            .Accepts<DeviceTypeCreateDTO>("application/json")
            .Produces<RestResponse>(201)
            .Produces(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.AdminPolicy);

        application.MapPut("/api/devicetypes/{id:int}", UpdateItemAsync)
            .WithName("Update Device Type")
            .Accepts<DeviceTypeUpdateDTO>("application/json")
            .Produces<RestResponse>(200)
            .Produces(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.AdminPolicy);

        /* User Policies */
        application.MapGet("/api/devicetypes/{id:int}", GetItemAsync)
           .WithName("Get Device Type")
           .Produces<RestResponse>(200)
           .Produces(401)
           .RequireAuthorization(ApplicationPolicies.UserPolicy);

    }

    public async Task<IResult> GetAllItemsAsync(IDeviceTypeService service)
    {
        var result = await service.GetAllAsync();
        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, result));
    }

    public async Task<IResult> GetItemAsync(IDeviceTypeService service, int id)
    {
        var result = await service.GetAsync(id);

        if (result != null)
            return Results.BadRequest(new RestResponse("Item is not existing."));

        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, result));
    }

    private async Task<IResult> UpdateItemAsync(IDeviceTypeService service,
                                                IValidator<DeviceTypeUpdateDTO> validation,
                                                int id,
                                                [FromBody] DeviceTypeUpdateDTO request)
    {

        var validationResult = await validation.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.BadRequest(new RestResponse(validationResult.Errors.FirstOrDefault().ErrorMessage));

        var device = request.UpdateDeviceType(id);

        var updatedDeviceType = await service.UpdateAsync(device);

        if (updatedDeviceType == null)
            return Results.BadRequest(new RestResponse("Device not updated. Item not found."));

        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, null));
    }

    private async Task<IResult> CreateItemAsync(IDeviceTypeService service,
                                                IValidator<DeviceTypeCreateDTO> validation,
                                                [FromBody] DeviceTypeCreateDTO request)
    {
        var validationResult = await validation.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.BadRequest(new RestResponse(validationResult.Errors.FirstOrDefault().ErrorMessage));

        var deviceType = request.CreateDeviceType();

        var createdDeviceType = await service.CreateAsync(deviceType);

        return Results.Created($"/api/device/{createdDeviceType.Id}", new RestResponse(HttpStatusCode.Created, true, null));
    }
}
