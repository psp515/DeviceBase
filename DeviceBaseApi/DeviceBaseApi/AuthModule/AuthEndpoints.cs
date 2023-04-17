using FluentValidation;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using DeviceBaseApi.AuthModule.DTO;
using DeviceBaseApi.Interfaces;
using DeviceBaseApi.Models;
using AutoMapper;

namespace DeviceBaseApi.AuthModule;

public class AuthEndpoints : IEndpoint
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
            .Produces(400);
    }

    private async Task<IResult> Register(IAuthService _authRepo, IMapper mapper, [FromBody] RegisterRequestDTO request)
    {
        bool userExists = await _authRepo.UserExists(request.Email);

        if (userExists)
            return Results.BadRequest(new RestResponse("Email is used by other user."));

        var user = new User { Email = request.Email, UserName = request.UserName, NormalizedEmail = request.Email.ToUpper() };

        var registerResponse = await _authRepo.Register(user, request.Password);

        if (registerResponse.HasError)
            return Results.BadRequest(new RestResponse(registerResponse.Error));

        return Results.Created($"/api/user/{registerResponse.Value.Id}", new RestResponse(HttpStatusCode.Created, true, registerResponse.Value));
    }

    private async Task<IResult> Login(IAuthService _authRepo, [FromBody] LoginRequestDTO request)
    {
        var loginResponse = await _authRepo.Login(request.Email, request.Password);

        if (loginResponse.HasError)
             return Results.BadRequest(new RestResponse(loginResponse.Error));

        // FIX: Better is to send token in Header

        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, result:loginResponse.Value));
    }
}
