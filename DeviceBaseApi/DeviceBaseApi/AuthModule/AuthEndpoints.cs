using Azure.Core;
using DeviceBaseApi.AuthModule.DTO;
using DeviceBaseApi.Interfaces;
using DeviceBaseApi.Models;
using DeviceBaseApi.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DeviceBaseApi.AuthModule;

public class AuthEndpoints : IEndpoints
{
    public void Configure(WebApplication app)
    {

        app.MapPost("/api/auth/login", Login)
            .WithName("Login")
            .Accepts<LoginRequestDTO>("application/json")
            .Produces<RestResponse>(200)
            .Produces(400);

        app.MapPost("/api/auth/register", Register)
            .WithName("Register")
            .Accepts<RegisterRequestDTO>("application/json")
            .Produces<RestResponse>(201)
            .Produces<RestResponse>(400);

        app.MapPost("/api/auth/refresh", RefreshTokens)
            .WithName("Refresh tokens")
            .Produces<RestResponse>(200)
            .Produces<RestResponse>(400);
    }

    private async Task<IResult> Register(IAuthService authRepo, IConfiguration config, [FromBody] RegisterRequestDTO request)
    {
        bool userExists = await authRepo.UserExists(request.Email);

        if (userExists)
            return Results.BadRequest(new RestResponse("Email is used by other user."));

        var user = request.CreateUser(config.GetValue<string>("DefaultUserSettings:ImageUrl"));

        var registerResponse = await authRepo.Register(user, request.Password);

        if (!registerResponse.Success)
            return Results.BadRequest(new RestResponse(registerResponse.Error));

        return Results.Created($"/api/user/{registerResponse.Value}", new RestResponse(HttpStatusCode.Created, true, registerResponse.Value));
    }

    private async Task<IResult> Login(IAuthService authRepo, [FromBody] LoginRequestDTO request)
    {
        var loginResponse = await authRepo.Login(request.Email, request.Password);

        if (!loginResponse.Success)
            return Results.BadRequest(new RestResponse(loginResponse.Error));

        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, loginResponse.Value));

    }

    private async Task<IResult> RefreshTokens(IAuthService authRepo,
                                              IConfiguration configuration,
                                              [FromHeader(Name = "Refresh")] string bearerRefreshToken,
                                              [FromHeader(Name = "Authorization")] string bearerToken)
    {
        var guid = bearerToken.GetValueFromToken(configuration.GetValue<string>("ApiSettings:Secret"));

        var loginResponse = await authRepo.RefreshTokens(bearerRefreshToken, guid);

        if (!loginResponse.Success)
            return Results.BadRequest(new RestResponse(loginResponse.Error));

        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, loginResponse.Value));
    }

}
