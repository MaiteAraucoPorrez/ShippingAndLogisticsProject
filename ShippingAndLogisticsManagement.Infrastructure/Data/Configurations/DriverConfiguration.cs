using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Infrastructure.Data.Configurations
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.ToTable("Drivers");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.IdentityDocument)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(d => d.IdentityDocument)
                .IsUnique()
                .HasDatabaseName("IX_Driver_IdentityDocument_Unique");

            builder.Property(d => d.LicenseNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(d => d.LicenseNumber)
                .IsUnique()
                .HasDatabaseName("IX_Driver_LicenseNumber_Unique");

            builder.Property(d => d.LicenseCategory)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(d => d.LicenseIssueDate)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(d => d.LicenseExpiryDate)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(d => d.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(d => d.AlternativePhone)
                .HasMaxLength(20);

            builder.Property(d => d.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(d => d.Email)
                .HasDatabaseName("IX_Driver_Email");

            builder.Property(d => d.Address)
                .HasMaxLength(300);

            builder.Property(d => d.City)
                .HasMaxLength(100);

            builder.Property(d => d.DateOfBirth)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(d => d.HireDate)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(d => d.ContractEndDate)
                .HasColumnType("date");

            builder.Property(d => d.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(d => d.AverageRating)
                .HasColumnType("decimal(3,2)");

            builder.Property(d => d.EmergencyContactName)
                .HasMaxLength(100);

            builder.Property(d => d.EmergencyContactPhone)
                .HasMaxLength(20);

            builder.Property(d => d.BloodType)
                .HasMaxLength(5);

            builder.Property(d => d.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(d => d.Notes)
                .HasMaxLength(500);

            // Relación con Vehicle
            builder.HasOne(d => d.CurrentVehicle)
                .WithOne(v => v.AssignedDriver)
                .HasForeignKey<Driver>(d => d.CurrentVehicleId)
                .OnDelete(DeleteBehavior.SetNull);

            // Índices para mejorar rendimiento
            builder.HasIndex(d => d.Status);
            builder.HasIndex(d => d.IsActive);
            builder.HasIndex(d => d.LicenseExpiryDate);
            builder.HasIndex(d => new { d.IsActive, d.Status })
                .HasDatabaseName("IX_Driver_Active_Status");
        }
    }
}
