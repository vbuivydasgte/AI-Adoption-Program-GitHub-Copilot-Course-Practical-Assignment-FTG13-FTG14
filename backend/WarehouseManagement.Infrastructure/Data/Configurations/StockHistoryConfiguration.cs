using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

public class StockHistoryConfiguration : IEntityTypeConfiguration<StockHistory>
{
    public void Configure(EntityTypeBuilder<StockHistory> builder)
    {
        builder.HasKey(sh => sh.Id);

        builder.Property(sh => sh.StockId)
            .IsRequired();

        builder.Property(sh => sh.ProductId)
            .IsRequired();

        builder.Property(sh => sh.WarehouseId)
            .IsRequired();

        builder.Property(sh => sh.ChangedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(sh => sh.ChangedAt)
            .IsRequired();

        builder.Property(sh => sh.PreviousQuantity)
            .IsRequired();

        builder.Property(sh => sh.NewQuantity)
            .IsRequired();

        builder.Property(sh => sh.QuantityChange)
            .IsRequired();

        builder.Property(sh => sh.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(sh => sh.StockId);
        builder.HasIndex(sh => sh.ProductId);
        builder.HasIndex(sh => sh.WarehouseId);
        builder.HasIndex(sh => sh.ChangedAt);

        builder.HasOne(sh => sh.Stock)
            .WithMany(s => s.StockHistories)
            .HasForeignKey(sh => sh.StockId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
