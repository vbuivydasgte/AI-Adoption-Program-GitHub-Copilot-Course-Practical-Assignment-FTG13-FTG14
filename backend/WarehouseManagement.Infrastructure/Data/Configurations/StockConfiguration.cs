using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.ProductId)
            .IsRequired();

        builder.Property(s => s.WarehouseId)
            .IsRequired();

        builder.Property(s => s.Quantity)
            .IsRequired();

        builder.Property(s => s.LastUpdated)
            .IsRequired();

        // Unique constraint: one product per warehouse
        builder.HasIndex(s => new { s.ProductId, s.WarehouseId })
            .IsUnique();

        builder.HasOne(s => s.Product)
            .WithMany(p => p.Stocks)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Warehouse)
            .WithMany(w => w.Stocks)
            .HasForeignKey(s => s.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.StockHistories)
            .WithOne(sh => sh.Stock)
            .HasForeignKey(sh => sh.StockId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
