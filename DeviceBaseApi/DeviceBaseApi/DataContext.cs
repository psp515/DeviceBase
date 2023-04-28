using DeviceBaseApi.AuthModule;
using DeviceBaseApi.DeviceModule;
using DeviceBaseApi.DeviceTypeModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DeviceBaseApi;

public class DataContext : IdentityDbContext<User>
{

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<User> AppUsers { get; set; }

    public DbSet<Device> Devices { get; set; }
    public DbSet<DeviceType> DeviceTypes { get; set; }

    public DbSet<T> GetDbSet<T>() where T : BaseModel
    {
        if (typeof(T) == typeof(Device))
            return Devices as DbSet<T>;

        if (typeof(T) == typeof(DeviceType))
            return DeviceTypes as DbSet<T>;

        throw new Exception($"DbSet not found for object: {nameof(T)}.");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var defaultDeviceType = new DeviceType()
        {
            Id = 1,
            Created = DateTime.Now,
            Edited = DateTime.Now,
            DefaultName = "SP611",
            MaximalNumberOfUsers = 5,
            EndpointsJson = "[\"state\",\"mode\",\"ping\"]"
        };

        modelBuilder.Entity<DeviceType>()
            .HasData(defaultDeviceType);

        modelBuilder.Entity<Device>()
            .HasData(new Device()
            {
                Id = 1,
                Created = DateTime.Now,
                Edited = DateTime.Now,

                Produced = DateTime.Now,
                MqttUrl = "https://www.google.pl/",
                SerialNumber = "21371",
                DeviceType = defaultDeviceType,
                DeviceName = "SP611",
                DevicePlacing = "None",
                Description = "",
                Users = new List<User>()
            },
            new Device()
            {
                Id = 2,
                Created = DateTime.Now,
                Edited = DateTime.Now,

                Produced = DateTime.Now,
                MqttUrl = "https://www.hivemq.com/",
                SerialNumber = "21372",
                DeviceType = defaultDeviceType,
                DeviceName = "SP611",
                DevicePlacing = "None",
                Description = "",
                Users = new List<User>()
            });

        
    }
}
