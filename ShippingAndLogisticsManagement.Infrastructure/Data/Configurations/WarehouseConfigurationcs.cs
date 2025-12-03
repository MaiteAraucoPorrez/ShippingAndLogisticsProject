using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Infrastructure.Data.Configurations
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable("Warehouses");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(w => w.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(w => w.Code)
                .IsUnique()
                .HasDatabaseName("IX_Warehouse_Code_Unique");

            builder.Property(w => w.Address)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(w => w.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(w => w.Department)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(w => w.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(w => w.Email)
                .HasMaxLength(100);

            builder.Property(w => w.MaxCapacityM3)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(w => w.CurrentCapacityM3)
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0);

            builder.Property(w => w.Type)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(w => w.OperatingHours)
                .HasMaxLength(200);

            builder.Property(w => w.ManagerName)
                .HasMaxLength(100);

            // Índices para búsquedas frecuentes
            builder.HasIndex(w => w.City);
            builder.HasIndex(w => w.Department);
            builder.HasIndex(w => w.Type);
            builder.HasIndex(w => w.IsActive);
        }
    }
}