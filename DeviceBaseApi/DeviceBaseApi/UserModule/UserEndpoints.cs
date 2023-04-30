using DeviceBaseApi.Interfaces;
using DeviceBaseApi.Models;
using DeviceBaseApi.UserModule.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DeviceBaseApi.UserModule;

public class UserEndpoints : IEndpoints
{
    public void Configure(WebApplication application)
    {
        application.MapGet("/api/user/settings", GetUserSettings)
           .WithName("Get User Settings")
           .Produces<RestResponse>(200)
           .Produces(401)
           .RequireAuthorization(ApplicationPolicies.UserPolicy);

        application.MapPut("/api/user/settings", UpdateUserSettings)
            .WithName("Update User Settings")
            .Accepts<UserSettingsDTO>("application/json")
            .Produces(204)
            .Produces<string>(400)
            .Produces(401)
            .RequireAuthorization(ApplicationPolicies.UserPolicy);
    }


    private async Task<IResult> GetUserSettings(IUserService service,
                                                IConfiguration configuration,
                                                [FromHeader(Name = "Authorization")] string bearerToken) 
    {
        var guid = bearerToken.GetValueFromToken(configuration.GetValue<string>("ApiSettings:Secret"));

        if (guid == null)
            return Results.BadRequest("Invalid token.");

        var result = await service.GetUser(guid);

        if (result == null)
            return Results.BadRequest(new RestResponse("User not found."));

        var dto = result.ToUserDTO();

        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, dto));
    }

    private async Task<IResult> UpdateUserSettings(IUserService service,
                                                   IConfiguration configuration,
                                                   IValidator<UserSettingsDTO> validation,
                                                   [FromBody] UserSettingsDTO request,
                                                   [FromHeader(Name = "Authorization")] string bearerToken)
    {
        var guid = bearerToken.GetValueFromToken(configuration.GetValue<string>("ApiSettings:Secret"));

        if (guid == null)
            return Results.BadRequest("Invalid token.");


        var validationResult = await validation.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.BadRequest(new RestResponse(validationResult.Errors.FirstOrDefault().ErrorMessage));

        var user = await service.GetUser(guid);

        if (user == null)
            return Results.BadRequest("User not found.");

        user.UpdateUserSettings(request);

        var updatedUser = await service.UpdateUser(user);

        if (updatedUser == null)
            return Results.BadRequest(new RestResponse("Device not updated. Item not found."));

        return Results.NoContent();
    }
}
