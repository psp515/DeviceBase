using DeviceBaseApi.UserModule;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;


using DeviceBaseApi.AuthModule;
using System.Net.Http.Json;
using System.Net;
using DeviceBaseApi.AuthModule.DTO;
using DeviceBaseApi.Models;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;

namespace DeviceBaseApiTests.AuthModule;

public class AuthEndpointTests
{
    public async Task<WebApplicationFactory<Program>> CreateApp()
    {
        await using var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder
            .ConfigureServices(services =>
            {
                services.AddScoped<IAuthService, TestAuthService>();
            }));

        return application;
    }

    [Fact]
    public async Task FailLoginTest() 
    {
        using var client = (await CreateApp()).CreateClient();

        var result = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDTO { Email="psp515@wp.pl", Password="Psp123!"}) ;

        var content = await result.Content.ReadFromJsonAsync<RestResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task SuccesLoginTest()
    {
        using var client = (await CreateApp()).CreateClient();

        var result = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDTO { Email="psp515@wp.pl", Password="Psp515!" });

        var content = await result.Content.ReadFromJsonAsync<RestResponse>();

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }


    [Fact]
    public async Task SuccessRegisterTest()
    {
        using var client = (await CreateApp()).CreateClient();

        var result = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequestDTO 
        { 
            Email="new@wp.pl", 
            Password="Psp123!",
            ConfirmPassword="Psp123!",
            UserName="username"
        });


        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task FailRegisterTest()
    {
        using var client = (await CreateApp()).CreateClient();

        var result = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequestDTO { Email="psp515@wp.pl", Password="Psp123!" });

        var content = await result.Content.ReadFromJsonAsync<RestResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }


    [Fact]
    public async Task SuccesRefreshTokensTest()
    {
        using var client = (await CreateApp()).CreateClient();

        var loginResult = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDTO { Email="psp515@wp.pl", Password="Psp515!" });

        var content = await loginResult.Content.ReadFromJsonAsync<RestResponse>();

        Assert.Equal(HttpStatusCode.OK, loginResult.StatusCode);
        Assert.True(content?.IsSuccess);

        LoginResponseDTO tokens = JsonConvert.DeserializeObject<LoginResponseDTO>(content.Result.ToString());

        client.DefaultRequestHeaders.Add("Authorization", tokens?.AccessToken);
        client.DefaultRequestHeaders.Add("Refresh", tokens?.RefreshToken);

        var refreshResult = await client.PostAsync("/api/auth/refresh", null);

        Assert.Equal(HttpStatusCode.OK, refreshResult.StatusCode);
    }

    [Fact]
    public async Task FailRefreshTokensTest()
    {
        using var client = (await CreateApp()).CreateClient();

        var loginResult = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDTO { Email="psp515@wp.pl", Password="Psp515!" });

        var content = await loginResult.Content.ReadFromJsonAsync<RestResponse>();

        Assert.Equal(HttpStatusCode.OK, loginResult.StatusCode);
        Assert.True(content?.IsSuccess);

        LoginResponseDTO tokens = JsonConvert.DeserializeObject<LoginResponseDTO>(content.Result.ToString());

        client.DefaultRequestHeaders.Add("Authorization", tokens?.AccessToken);
        client.DefaultRequestHeaders.Add("Refresh", tokens?.RefreshToken + "123");

        var refreshResult = await client.PostAsync("/api/auth/refresh", null);

        Assert.Equal(HttpStatusCode.BadRequest, refreshResult.StatusCode);
    }
}
