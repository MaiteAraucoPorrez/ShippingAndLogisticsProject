using Microsoft.EntityFrameworkCore;
using ShippingAndLogisticsManagement.Core.Entities;
using System.Reflection;

namespace ShippingAndLogisticsManagement.Infrastructure.Data
{
    public class LogisticContext : DbContext
    {
        public LogisticContext(DbContextOptions<LogisticContext> options) 
            : base(options) 
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Shipment> Shipments { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<Route> Routes { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }
        public virtual DbSet<Driver> Drivers { get; set; }
        public virtual DbSet<ShipmentWarehouse> ShipmentWarehouses { get; set; }
        public virtual DbSet<Security> Securities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
