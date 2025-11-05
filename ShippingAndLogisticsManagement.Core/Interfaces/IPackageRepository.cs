using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    public interface IPackageRepository: IBaseRepository<Package>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        Task<IEnumerable<Package>> GetAllAsync(int CustomerId);

        // <summary>
        /// Gets all the packages from a specific shipment using Dapper
        /// </summary>
        Task<IEnumerable<Package>> GetByShipmentIdDapperAsync(int shipmentId);

        /// <summary>
        /// Obtains a statistical summary of packages per shipment (Dapper) 
        /// Includes: Total packages, total weight, total value, averages
        /// </summary>
        Task<PackageSummaryResponse> GetPackageSummaryAsync(int shipmentId);

        /// <summary>
        /// Gets all packages with Dapper (for GETs)
        /// </summary>
        Task<IEnumerable<Package>> GetAllDapperAsync(int limit = 10);

        /// <summary>
        /// Gets a package by ID using Dapper
        /// </summary>
        Task<Package> GetByIdDapperAsync(int id);

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
