using DeviceBaseApi.AuthModule;
using DeviceBaseApi.DeviceModule;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Device>().HasData(
            new Device()
            {
                DeviceId = 1,
                Users = new List<User>(),
                Created = DateTime.Now,
                Edited = DateTime.Now,
                MqttUrl = "https://www.google.pl/",
                SerialNumber = "1094205034"
            });
    }
}
