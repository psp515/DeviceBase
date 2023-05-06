
using DeviceBaseApi.AuthModule;
using DeviceBaseApi.AuthModule.DTO;
using DeviceBaseApi.DeviceModule;
using DeviceBaseApi.DeviceModule.DTO;
using DeviceBaseApi.DeviceTypeModule;
using DeviceBaseApi.DeviceTypeModule.DTO;
using DeviceBaseApi.Models;
using DeviceBaseApiTests.AuthModule;
using DeviceBaseApiTests.DeviceTypeModule;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace DeviceBaseApiTests.DeviceModule;

public class DeviceEndpointTest
{
    public async Task<WebApplicationFactory<Program>> CreateApp()
    {
        await using var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder
            .ConfigureServices(services =>
            {
                services.AddScoped<IAuthService, TestAuthService>();
                services.AddScoped<IDeviceService, TestDeviceService>();
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

        var result = await client.PostAsync("/api/devices", null);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }
    [Fact]
    public async Task ForbiddenCreate()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await Login(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.PostAsync("/api/devices", null);

        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
    }
    [Fact]
    public async Task Create()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await LoginAdmin(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        DeviceCreateDTO dto = new DeviceCreateDTO 
        { 
            DeviceTypeId = 1,
            SerialNumber="1352345", 
            MqttUrl="https://www.google.com", 
            Produced = DateTime.Now.AddDays(-1) 
        };

        var result = await client.PostAsJsonAsync<DeviceCreateDTO>("/api/devices", dto);

        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task UnauthorisedGetAll()
    {
        using var client = (await CreateApp()).CreateClient();

        var result = await client.GetAsync("/api/devices");

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }
    [Fact]
    public async Task ForbiddenGetAll()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await Login(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.GetAsync("/api/devices");

        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
    }
    [Fact]
    public async Task GetAll()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await LoginAdmin(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.GetAsync("/api/devices");

        var content = await result.Content.ReadFromJsonAsync<RestResponse>();

        List<DeviceType> dto = JsonConvert.DeserializeObject<List<DeviceType>>(content.Result.ToString());

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(2, dto.Count);
    }

    [Fact]
    public async Task UnauthorisedUpdate()
    {
        using var client = (await CreateApp()).CreateClient();

        var result = await client.PutAsync("/api/devices/1", null);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }
    [Fact]
    public async Task NotConnectedUpdate()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await Login(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        DeviceUpdateDTO dto = new DeviceUpdateDTO {
        Description = "Description sasda.",
        DeviceName = "Device name",
        DevicePlacing = "Kitchemnsdf"};

        var result = await client.PutAsJsonAsync<DeviceUpdateDTO>("/api/devices/1", dto);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
    [Fact]
    public async Task Update()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await LoginAdmin(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        DeviceUpdateDTO dto = new DeviceUpdateDTO
        {
            Description = "Description sasda.",
            DeviceName = "Device name",
            DevicePlacing = "Kitchemnsdf"
        };

        var result = await client.PutAsJsonAsync<DeviceUpdateDTO>("/api/devices/1", dto);

        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }


    [Fact]
    public async Task UnauthorisedGet()
    {
        using var client = (await CreateApp()).CreateClient();

        var result = await client.GetAsync("/api/devices/1");

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }
    [Fact]
    public async Task ForbiddenGet()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await Login(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.GetAsync("/api/devices/1");

        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
    }
    [Fact]
    public async Task GetAdmin()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await LoginAdmin(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.GetAsync("/api/devices/1");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task UnauthorisedGetItems()
    {
        using var client = (await CreateApp()).CreateClient();

        var result = await client.GetAsync("/api/devices/user");

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }
    [Fact]
    public async Task GetItemsAdmin()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await LoginAdmin(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.GetAsync("/api/devices/user");

        var content = await result.Content.ReadFromJsonAsync<RestResponse>();

        List<Device> dto = JsonConvert.DeserializeObject<List<Device>>(content.Result.ToString());

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(1, dto?.Count);
    }
    [Fact]
    public async Task GetItemsUser()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await Login(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.GetAsync("/api/devices/user");

        var content = await result.Content.ReadFromJsonAsync<RestResponse>();

        List<Device> dto = JsonConvert.DeserializeObject<List<Device>>(content.Result.ToString());

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(0, dto?.Count);
    }

    [Fact]
    public async Task UnauthorisedConnect()
    {
        using var client = (await CreateApp()).CreateClient();

        var result = await client.PatchAsync("/api/devices/1/connect", null);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task AlreadyConnected()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await LoginAdmin(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.PatchAsync("/api/devices/1/connect", null);

        var content = await result.Content.ReadFromJsonAsync<RestResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Contains("Connection already exists.", content?.ErrorMessage);
    }

    [Fact]
    public async Task TooMuchConnections()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await Login(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.PatchAsync("/api/devices/1/connect", null);

        var content = await result.Content.ReadFromJsonAsync<RestResponse>();
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Contains("Cannot connect more devices", content?.ErrorMessage);
    }

    [Fact]
    public async Task SuccessConnection()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await Login(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.PatchAsync("/api/devices/2/connect", null);

        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task SuccessDisconnection()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await LoginAdmin(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.PatchAsync("/api/devices/1/disconnect", null);

        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task CannotDisconnect()
    {
        using var client = (await CreateApp()).CreateClient();

        var token = await Login(client);

        client.DefaultRequestHeaders.Add("Authorization", token);

        var result = await client.PatchAsync("/api/devices/1/disconnect", null);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
}
