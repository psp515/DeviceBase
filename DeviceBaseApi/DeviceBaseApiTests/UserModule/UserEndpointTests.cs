using DeviceBaseApi.AuthModule;
using DeviceBaseApi.AuthModule.DTO;
using DeviceBaseApi.Models;
using DeviceBaseApi.UserModule;
using DeviceBaseApi.UserModule.DTO;
using DeviceBaseApi.Utils;
using DeviceBaseApiTests.AuthModule;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace DeviceBaseApiTests.UserModule;

public class UserEndpointTests
{
    public async Task<WebApplicationFactory<Program>> CreateApp()
    {
        await using var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder
            .ConfigureServices(services =>
            {
                services.AddScoped<IAuthService, TestAuthService>();
                services.AddScoped<IUserService, TestUserService>();
            }));

        return application;
    }

    public async Task<string> Login(HttpClient client)
    {
        var result = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDTO { Email="psp515@wp.pl", Password="Psp515!" });
        var content = await result.Content.ReadFromJsonAsync<RestResponse>();
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        LoginResponseDTO tokens = JsonConvert.DeserializeObject<LoginResponseDTO>(content.Result.ToString());

        return tokens?.AccessToken;
    }

    [Fact]
    public async Task UnauthorisedUserData()
    {
        using var client = (await CreateApp()).CreateClient();

        var result = await client.GetAsync("/api/user/settings");

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task GetValidUserData() 
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await Login(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.GetAsync("/api/user/settings");

        var content = await result.Content.ReadFromJsonAsync<RestResponse>();

        UserDTO dto = JsonConvert.DeserializeObject<UserDTO>(content.Result.ToString());

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("psp515", dto?.UserName);
    }

    [Fact]
    public async Task UnauthorisedSetUserData()
    {
        using var client = (await CreateApp()).CreateClient();

        var dto = new UserSettingsDTO
            (AppModeEnum.Light,
            LanguageEnum.English,
            "https://www.google.com",
            "534534345",
            true,
        false,
        false
        );

        var data = JsonConvert.SerializeObject(dto);

        var result = await client.PutAsJsonAsync("/api/user/settings", data);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }


    [Fact]
    public async Task SetValidUserData()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await Login(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var data = new UserSettingsDTO(
            AppModeEnum.Universal, 
            LanguageEnum.English, 
            "https://localhost:7101/swagger/index.html", 
            "+11-111-111-111", 
            true, 
            true,
            true);

        var result = await client.PutAsJsonAsync<UserSettingsDTO>("/api/user/settings", data);

        var content2 = await result.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);

        result = await client.GetAsync("/api/user/settings");

        var content = await result.Content.ReadFromJsonAsync<RestResponse>();

        UserDTO dto = JsonConvert.DeserializeObject<UserDTO>(content.Result.ToString());

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(AppModeEnum.Universal, dto?.AppMode);
        Assert.Equal(LanguageEnum.English, dto?.Language);
    }
}
