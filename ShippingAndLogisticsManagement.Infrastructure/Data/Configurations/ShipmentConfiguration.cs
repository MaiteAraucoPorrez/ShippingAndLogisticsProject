using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Infrastructure.Data.Configurations
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.ShippingDate)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(s => s.State)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(s => s.TotalCost)
                .HasColumnType("float")
                .IsRequired();

            builder.Property(s => s.TrackingNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(s => s.Customer)
                .WithMany(c => c.Shipments)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasMany(s => s.Packages)
                .WithOne(p => p.Shipment)
                .HasForeignKey(p => p.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
