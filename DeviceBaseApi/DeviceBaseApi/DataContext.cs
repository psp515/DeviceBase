using DeviceBaseApi.AuthModule;
using DeviceBaseApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DeviceBaseApi;

public class DataContext : DbContext
{ 
	public DataContext(DbContextOptions<DataContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Here we can add default data pushed after migration
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Device> Devices { get; set; }
}


