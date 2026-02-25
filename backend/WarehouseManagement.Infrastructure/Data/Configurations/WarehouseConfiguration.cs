using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Infrastructure.Data.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Location)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(w => w.CreatedAt)
            .IsRequired();

        builder.Property(w => w.ModifiedAt)
            .IsRequired();

        builder.HasMany(w => w.Stocks)
            .WithOne(s => s.Warehouse)
            .HasForeignKey(s => s.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
