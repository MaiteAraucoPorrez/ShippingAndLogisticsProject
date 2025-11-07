using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    public interface IPackageService
    {
        Task<ResponseData> GetAllAsync(PackageQueryFilter packageQueryFilter);
        Task<IEnumerable<Package>> GetAllDapperAsync();
        Task<PackageSummaryResponse> GetPackageSummaryAsync(int shipmentId);
        Task<Package> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene un paquete por ID usando Dapper
        /// </summary>
        Task<Package> GetByIdDapperAsync(int id);
        Task InsertAsync(Package package);
        Task UpdateAsync(Package package);
        Task DeleteAsync(int id);
    }
}
