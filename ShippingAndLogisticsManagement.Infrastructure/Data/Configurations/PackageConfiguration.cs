using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Infrastructure.Data.Configurations
{
    public class PackageConfiguration : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Weight)
                .HasColumnType("float")
                .IsRequired();

            builder.Property(p => p.Price)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.HasOne(p => p.Shipment)
                .WithMany(s => s.Packages)
                .HasForeignKey(p => p.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
