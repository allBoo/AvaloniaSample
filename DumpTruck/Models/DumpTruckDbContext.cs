using System;
using Avalonia.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DumpTruck.Models;

public class DumpTruckDbContext : DbContext
{
    public DbSet<Garage<IVehicle>> Garages { get; set; }
    
    public DbSet<DumpTruck> DumpTrucks { get; set; }
    
    public DbSet<TipTruck> TipTrucks { get; set; }
    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("/apps/ulstu/DumpTruck/DumpTruck/appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        
        Console.WriteLine("CONFIG " + configuration.GetSection("ConnectionString").GetValue<string>("DumpTruck"));
        
        var connectionString = configuration.GetSection("ConnectionString").GetValue<string>("DumpTruck");
        
        optionsBuilder.UseNpgsql(connectionString, options => 
            options.SetPostgresVersion(new Version(9, 6)));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<DumpTruck>()
            .Property(e => e.BodyColor)
            .HasConversion(
                v => v.ToString(),
                v => Color.Parse(v));
        
        modelBuilder
            .Entity<TipTruck>()
            .Property(e => e.TipperColor)
            .HasConversion(
                v => v.ToString(),
                v => Color.Parse(v));
        
        modelBuilder
            .Entity<TipTruck>()
            .Property(e => e.TentColor)
            .HasConversion(
                v => v.ToString(),
                v => Color.Parse(v));
    }
}
