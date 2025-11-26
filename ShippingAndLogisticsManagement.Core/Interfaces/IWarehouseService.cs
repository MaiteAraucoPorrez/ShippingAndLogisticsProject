using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del servicio para la gestión de almacenes
    /// </summary>
    public interface IWarehouseService
    {
        Task<ResponseData> GetAllAsync(WarehouseQueryFilter filters);
        Task<IEnumerable<Warehouse>> GetAllDapperAsync();
        Task<Warehouse> GetByIdAsync(int id);
        Task<Warehouse> GetByIdDapperAsync(int id);
        Task<Warehouse> GetByCodeAsync(string code);
        Task<IEnumerable<Warehouse>> GetActiveWarehousesAsync();
        Task<IEnumerable<Warehouse>> GetAvailableWarehousesAsync(double requiredCapacity);
        Task<WarehouseStatisticsResponse> GetWarehouseStatisticsAsync(int warehouseId);
        Task InsertAsync(Warehouse warehouse);
        Task UpdateAsync(Warehouse warehouse);
        Task DeleteAsync(int id);
        Task DeactivateAsync(int id);
        Task UpdateCapacityAsync(int warehouseId, double capacityChange);
    }
}