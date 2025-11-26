using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Infrastructure.Data.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Department)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Zone)
                .HasMaxLength(100);

            builder.Property(a => a.PostalCode)
                .HasMaxLength(20);

            builder.Property(a => a.Reference)
                .HasMaxLength(500);

            builder.Property(a => a.Alias)
                .HasMaxLength(50);

            builder.Property(a => a.ContactName)
                .HasMaxLength(100);

            builder.Property(a => a.ContactPhone)
                .HasMaxLength(20);

            builder.Property(a => a.Type)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(a => a.Latitude)
                .HasColumnType("decimal(10,7)");

            builder.Property(a => a.Longitude)
                .HasColumnType("decimal(10,7)");

            builder.Property(a => a.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()");

            // Relación con Customer
            builder.HasOne(a => a.Customer)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices para mejorar rendimiento
            builder.HasIndex(a => a.CustomerId);
            builder.HasIndex(a => a.City);
            builder.HasIndex(a => a.Department);
            builder.HasIndex(a => new { a.CustomerId, a.Type, a.IsDefault })
                .HasDatabaseName("IX_Address_Customer_Type_Default");
        }
    }
}