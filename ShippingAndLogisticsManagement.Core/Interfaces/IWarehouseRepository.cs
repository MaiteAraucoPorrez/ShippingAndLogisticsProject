using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Enum;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del repositorio para la entidad <see cref="Warehouse"/>.
    /// </summary>
    public interface IWarehouseRepository : IBaseRepository<Warehouse>
    {
        /// <summary>
        /// Obtiene todos los almacenes con filtros opcionales usando Dapper
        /// </summary>
        Task<IEnumerable<Warehouse>> GetAllDapperAsync(WarehouseQueryFilter filters);

        /// <summary>
        /// Obtiene un almacén por ID usando Dapper
        /// </summary>
        Task<Warehouse> GetByIdDapperAsync(int id);

        /// <summary>
        /// Obtiene un almacén por su código único
        /// </summary>
        Task<Warehouse> GetByCodeAsync(string code);

        /// <summary>
        /// Obtiene todos los almacenes activos
        /// </summary>
        Task<IEnumerable<Warehouse>> GetActiveWarehousesAsync();

        /// <summary>
        /// Obtiene almacenes por ciudad
        /// </summary>
        Task<IEnumerable<Warehouse>> GetByCityAsync(string city);

        /// <summary>
        /// Obtiene almacenes por departamento
        /// </summary>
        Task<IEnumerable<Warehouse>> GetByDepartmentAsync(string department);

        /// <summary>
        /// Obtiene almacenes por tipo
        /// </summary>
        Task<IEnumerable<Warehouse>> GetByTypeAsync(WarehouseType type);

        /// <summary>
        /// Obtiene almacenes con disponibilidad de capacidad
        /// </summary>
        Task<IEnumerable<Warehouse>> GetAvailableWarehousesAsync(double requiredCapacity);

        /// <summary>
        /// Obtiene estadísticas de un almacén
        /// </summary>
        Task<WarehouseStatisticsResponse> GetWarehouseStatisticsAsync(int warehouseId);

        /// <summary>
        /// Verifica si un código de almacén ya existe
        /// </summary>
        Task<bool> CodeExistsAsync(string code, int? excludeWarehouseId = null);

        /// <summary>
        /// Actualiza la capacidad actual de un almacén
        /// </summary>
        Task UpdateCurrentCapacityAsync(int warehouseId, double newCapacity);

        /// <summary>
        /// Obtiene los almacenes más recientes
        /// </summary>
        Task<IEnumerable<Warehouse>> GetRecentWarehousesAsync(int limit = 10);
    }
}