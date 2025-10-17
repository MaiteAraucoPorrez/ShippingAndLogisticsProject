using Microsoft.EntityFrameworkCore;
using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Infrastructure.Data
{
    public class LogisticContext : DbContext
    {
        public LogisticContext(DbContextOptions<LogisticContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Route> Routes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LogisticContext).Assembly);
        }
    }
}
