

using DeviceBaseApi.AuthModule;
using DeviceBaseApi.AuthModule.DTO;
using DeviceBaseApi.DeviceTypeModule;
using DeviceBaseApi.DeviceTypeModule.DTO;
using DeviceBaseApi.Models;
using DeviceBaseApiTests.AuthModule;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace DeviceBaseApiTests.DeviceTypeModule;

public class DeviceTypeEndpointTest
{
    public async Task<WebApplicationFactory<Program>> CreateApp()
    {
        await using var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder
            .ConfigureServices(services =>
            {
                services.AddScoped<IAuthService, TestAuthService>();
                services.AddScoped<IDeviceTypeService, TestDeviceTypeService>();
            }));

        return application;
    }

    public async Task<string?> LoginAdmin(HttpClient client)
    {
        var result = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDTO { Email="psp515@wp.pl", Password="Psp515!" });
        var content = await result.Content.ReadFromJsonAsync<RestResponse>();
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        LoginResponseDTO tokens = JsonConvert.DeserializeObject<LoginResponseDTO>(content.Result.ToString());

        return tokens?.AccessToken;
    }

    public async Task<string?> Login(HttpClient client)
    {
        var result = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDTO { Email="lul@wp.pl", Password="Psp515!" });
        var content = await result.Content.ReadFromJsonAsync<RestResponse>();
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        LoginResponseDTO tokens = JsonConvert.DeserializeObject<LoginResponseDTO>(content.Result.ToString());

        return tokens?.AccessToken;
    }

    [Fact]
    public async Task UnauthorisedCreate()
    {
        using var client = (await CreateApp()).CreateClient();

        var result = await client.PostAsync("/api/devicetypes", null);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task ForbiddenCreate()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await Login(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.PostAsync("/api/devicetypes", null);

        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task Create()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await LoginAdmin(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        DeviceTypeCreateDTO dto = new DeviceTypeCreateDTO("SP611F", 5, "[\"state\",\"mode\",\"ping\"]");

        var result = await client.PostAsJsonAsync<DeviceTypeCreateDTO>("/api/devicetypes", dto);

        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task UnauthorisedGetAll()
    {
        using var client = (await CreateApp()).CreateClient();

        var result = await client.GetAsync("/api/devicetypes");

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task ForbiddenGetAll()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await Login(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.GetAsync("/api/devicetypes");

        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task GetAll()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await LoginAdmin(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.GetAsync("/api/devicetypes");

        var content = await result.Content.ReadFromJsonAsync<RestResponse>();

        List<DeviceType> dto = JsonConvert.DeserializeObject<List<DeviceType>>(content.Result.ToString());

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(2, dto.Count);
    }

    [Fact]
    public async Task UnauthorisedUpdate()
    {
        using var client = (await CreateApp()).CreateClient();

        var result = await client.PutAsync("/api/devicetypes/1", null);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task ForbiddenUpdate()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await Login(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.PutAsync("/api/devicetypes/1", null);

        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task Update()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await LoginAdmin(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        DeviceTypeUpdateDTO dto = new DeviceTypeUpdateDTO("SPE611A");

        var result = await client.PutAsJsonAsync<DeviceTypeUpdateDTO>("/api/devicetypes/1", dto);

        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }


    [Fact]
    public async Task UnauthorisedGet()
    {
        using var client = (await CreateApp()).CreateClient();

        var result = await client.GetAsync("/api/devicetypes/1");

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task GetUser()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await Login(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.GetAsync("/api/devicetypes/1");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task GetAdmin()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await LoginAdmin(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.GetAsync("/api/devicetypes/1");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }
}
