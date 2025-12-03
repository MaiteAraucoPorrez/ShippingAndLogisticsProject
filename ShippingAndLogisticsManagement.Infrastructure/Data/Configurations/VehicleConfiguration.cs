using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Infrastructure.Data.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.PlateNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(v => v.PlateNumber)
                .IsUnique()
                .HasDatabaseName("IX_Vehicle_PlateNumber_Unique");

            builder.Property(v => v.Type)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(v => v.MaxWeightCapacityKg)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(v => v.MaxVolumeCapacityM3)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(v => v.CurrentWeightKg)
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0);

            builder.Property(v => v.CurrentVolumeM3)
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0);

            builder.Property(v => v.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(v => v.VIN)
                .HasMaxLength(17);

            builder.HasIndex(v => v.VIN)
                .IsUnique()
                .HasDatabaseName("IX_Vehicle_VIN_Unique")
                .HasFilter("[VIN] IS NOT NULL");

            // Relación con Warehouse
            builder.HasOne(v => v.BaseWarehouse)
                .WithMany()
                .HasForeignKey(v => v.BaseWarehouseId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relación con Driver
            builder.HasOne(v => v.AssignedDriver)
                .WithOne(d => d.CurrentVehicle)
                .HasForeignKey<Vehicle>(v => v.AssignedDriverId)
                .OnDelete(DeleteBehavior.SetNull);

            // Índices para mejorar rendimiento
            builder.HasIndex(v => v.Type);
            builder.HasIndex(v => v.Status);
            builder.HasIndex(v => v.IsActive);
            builder.HasIndex(v => v.BaseWarehouseId);
            builder.HasIndex(v => new { v.IsActive, v.Status })
                .HasDatabaseName("IX_Vehicle_Active_Status");
        }
    }
}