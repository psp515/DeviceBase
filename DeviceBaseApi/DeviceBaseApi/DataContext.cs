using DeviceBaseApi.AuthModule;
using DeviceBaseApi.Coupons;
using DeviceBaseApi.DeviceModule;
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
    public DbSet<Coupon> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Coupon>().HasData(
            new Coupon()
            {
                Id = 1,
                Name = "10OFF",
                Percent = 10,
                IsActive = true,
            },
            new Coupon()
            {
                Id = 2,
                Name = "20OFF",
                Percent = 20,
                IsActive = true,
            });


        modelBuilder.Entity<User>().HasData(
            new User()
            {
                Id = "1TestId",
                Email = "psp515@wp.pl",
                UserName = "psp515",
                PasswordHash = "",
            });



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
