using FluentValidation;
using DeviceBaseApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DeviceBaseApi.AuthModule;
using DeviceBaseApi.Interfaces;
using DeviceBaseApi.DeviceModule;
using DeviceBaseApi.DeviceTypeModule;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddIdentity<User, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<DataContext>();
builder.Services.AddSwaggerGen(option => {
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
             "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
             "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
             "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
builder.Services.AddDbContext<DataContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
            builder.Configuration.GetValue<string>("ApiSettings:Secret"))),
        ValidateIssuer = false,
        ValidateAudience = false

    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(ApplicationPolicies.AdminPolicy, policy => policy.RequireRole(new string[] { ApplicationRoles.Admin }));
    options.AddPolicy(ApplicationPolicies.UserPolicy, policy => policy.RequireRole(new string[] { ApplicationRoles.Admin, ApplicationRoles.AuthorizedUser }));
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IDeviceTypeService, DeviceTypeService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

var endpoints = new List<IEndpoint>
{
    new AuthEndpoints(),
    new DeviceEndpoints(),
    new DeviceTypeEndpoints()
};

endpoints.ForEach(endpoint => endpoint.Configure(app));

app.UseHttpsRedirection();

app.Run();


// admin@admin
// Admin123!