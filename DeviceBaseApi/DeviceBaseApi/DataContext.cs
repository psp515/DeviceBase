using DeviceBaseApi.AuthModule;
using DeviceBaseApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DeviceBaseApi;

public class DataContext : IdentityDbContext<User>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
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
    }
}
