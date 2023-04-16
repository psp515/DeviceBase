using FluentValidation;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using DeviceBaseApi.AuthModule.DTO;
using DeviceBaseApi.Interfaces;
using DeviceBaseApi.Models;

namespace DeviceBaseApi.AuthModule;

public class AuthEndpoints : IEndpoint
{
    public void Configure(WebApplication app)
    {

        app.MapPost("/api/login", Login)
            .WithName("Login")
            .Accepts<LoginRequestDTO>("application/json")
            .Produces<RestResponse>(200)
            .Produces(400);

        app.MapPost("/api/register", Register)
            .WithName("Register")
            .Accepts<RegisterRequestDTO>("application/json")
            .Produces<RestResponse>(201)
            .Produces(400);
    }

    private async Task<IResult> Register(IAuthService _authRepo, [FromBody] RegisterRequestDTO model)
    {
        bool userExists = await _authRepo.UserExists(model.Email);

        if (userExists)
            return Results.BadRequest(new RestResponse("Email is used by other user."));
        
        var registerResponse = await _authRepo.Register(model);

        if (registerResponse.HasError)
        {
            return Results.BadRequest(new RestResponse(registerResponse.Error));
        }

        // TODO: AuthorizedUser service

        return Results.Created($"/api/user/{registerResponse.Value.Id}", new RestResponse(HttpStatusCode.Created, true, registerResponse.Value));
    }

    private async Task<IResult> Login(IAuthService _authRepo, [FromBody] LoginRequestDTO model)
    {
        var loginResponse = await _authRepo.Login(model);

        if (loginResponse.HasError)
             return Results.BadRequest(new RestResponse(loginResponse.Error));

        // TODO: Better is to send token in Header

        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, result:loginResponse.Value));
    }
}
