using DeviceBaseApi.AuthModule.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DeviceBaseApi.AuthModule;

public static class AuthEndpoint
{
    public static void Configure(this WebApplication application)
    {

    }

    private async static Task<IResult> Register(IAuthService authService, IValidator<RegisterRequestDTO> validator, [FromBody] RegisterRequestDTO model)
    {
        var validationResult = await validator.ValidateAsync(model);

        if (validationResult.IsValid)
            return Results.BadRequest(new RestResponse("Invalid request data."));
        
        bool userExists = await authService.UserExists(model.Email);

        if (userExists)
            return Results.BadRequest(new RestResponse("Email is used by other user."));
        
        var registerResponse = await authService.Register(model);

        if (registerResponse != null)
            return Results.BadRequest(new RestResponse(registerResponse));

        return Results.Created("", new RestResponse(HttpStatusCode.Created, true, null));
    }

    private async static Task<IResult> Login(IAuthService authService, [FromBody] LoginRequestDTO model)
    {
        var loginResponse = await authService.Login(model);

        if (loginResponse == null)
            return Results.BadRequest(new RestResponse("Username or password is incorrect"));
       
        return Results.Ok(new RestResponse(HttpStatusCode.Created, true, loginResponse));
    }
}
