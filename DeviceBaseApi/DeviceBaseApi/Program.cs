using DeviceBaseApi;
using DeviceBaseApi.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//TODO: Change Database ?
// builder.Services.AddDbContext<DataContext>(options => options.UseSqlite("Datasource=TestDatabase"));
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.MapGet("api/r/{id:int}", (int id) =>
{

})
    .Produces<Device>(200)
.Accepts<Device>("application/json");

// Dokuemntacja to produces i accepts

app.MapPost("api/r", ([FromBody] Device device) =>
{

});

app.UseHttpsRedirection();

app.Run();


