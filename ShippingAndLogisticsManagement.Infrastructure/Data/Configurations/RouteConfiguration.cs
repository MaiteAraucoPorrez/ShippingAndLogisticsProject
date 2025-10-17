using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Infrastructure.Data.Configurations
{
    public class RouteConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Origin)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Destination)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.DistanceKm)
                .HasColumnType("float")
                .IsRequired();

            builder.Property(r => r.BaseCost)
                .HasColumnType("float")
                .IsRequired();
        }
    }
}
