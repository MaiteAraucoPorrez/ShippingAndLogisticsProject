using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    public interface IPackageRepository: IBaseRepository<Package>
    {
        Task<IEnumerable<Package>> GetAllAsync(int CustomerId);

        Task<IEnumerable<Package>> GetAllDapperAsync(int limit = 10);

        // <summary>
        /// Gets all the packages from a specific shipment using Dapper
        /// </summary>
        Task<IEnumerable<Package>> GetByShipmentIdDapperAsync(int shipmentId);

        /// <summary>
        /// Gets all packages with Dapper (for GETs)
        /// </summary>
        Task<IEnumerable<Package>> GetAllDapperAsync(PackageQueryFilter filters);

        /// <summary>
        /// Gets a package by ID using Dapper
        /// </summary>
        Task<Package> GetByIdDapperAsync(int id);

        /// <summary>
        /// Obtains a statistical summary of packages per shipment (Dapper) 
        /// Includes: Total packages, total weight, total value, averages
        /// </summary>
        Task<PackageSummaryResponse> GetPackageSummaryAsync(int shipmentId);

        /// <summary>
        /// Gets packages with complete information about the shipment, customer, and route (JOIN)
        /// </summary>
        Task<IEnumerable<PackageDetailResponse>> GetPackageWithDetailsAsync();

        /// <summary>
        /// Gets heavy packages (greater than a specific weight)
        /// </summary>
        Task<IEnumerable<Package>> GetHeavyPackagesAsync(double minWeight);

    }
}
