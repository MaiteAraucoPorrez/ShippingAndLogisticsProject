using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Infrastructure.Data.Configurations
{
    public class ShipmentWarehouseConfiguration : IEntityTypeConfiguration<ShipmentWarehouse>
    {
        public void Configure(EntityTypeBuilder<ShipmentWarehouse> builder)
        {
            builder.ToTable("ShipmentWarehouses");

            builder.HasKey(sw => sw.Id);

            builder.Property(sw => sw.EntryDate)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(sw => sw.ExitDate)
                .HasColumnType("datetime");

            builder.Property(sw => sw.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(sw => sw.ReceivedBy)
                .HasMaxLength(100);

            builder.Property(sw => sw.DispatchedBy)
                .HasMaxLength(100);

            builder.Property(sw => sw.Notes)
                .HasMaxLength(500);

            builder.Property(sw => sw.StorageLocation)
                .HasMaxLength(50);

            // Relaciones
            builder.HasOne(sw => sw.Shipment)
                .WithMany()
                .HasForeignKey(sw => sw.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sw => sw.Warehouse)
                .WithMany(w => w.ShipmentWarehouses)
                .HasForeignKey(sw => sw.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(sw => sw.ShipmentId);
            builder.HasIndex(sw => sw.WarehouseId);
            builder.HasIndex(sw => new { sw.ShipmentId, sw.WarehouseId });
            builder.HasIndex(sw => sw.Status);
        }
    }
}