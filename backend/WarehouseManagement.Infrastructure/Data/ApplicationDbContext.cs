using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<StockHistory> StockHistories { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Seed admin and worker users (plain text for migration)
        var staticDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "Admin123", // plain text for initial migration
                Role = "Admin",
                CreatedAt = staticDate
            },
            new User
            {
                Id = 2,
                Username = "worker",
                PasswordHash = "Worker123", // plain text for initial migration
                Role = "Worker",
                CreatedAt = staticDate
            }
        );
    }
}
