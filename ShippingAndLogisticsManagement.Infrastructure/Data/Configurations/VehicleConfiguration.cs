using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Infrastructure.Data.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.PlateNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(v => v.PlateNumber)
                .IsUnique();

            builder.Property(v => v.Brand)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.Model)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.Type)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(v => v.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasOne(v => v.BaseWarehouse)
                .WithMany()
                .HasForeignKey(v => v.BaseWarehouseId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(v => v.AssignedDriver)
                .WithOne(d => d.CurrentVehicle)
                .HasForeignKey<Vehicle>(v => v.AssignedDriverId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
