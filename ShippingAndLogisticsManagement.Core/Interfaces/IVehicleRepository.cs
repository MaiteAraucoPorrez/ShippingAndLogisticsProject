using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del repositorio para la entidad <see cref="Vehicle"/>.
    /// </summary>
    public interface IVehicleRepository : IBaseRepository<Vehicle>
    {
        /// <summary>
        /// Obtiene todos los vehículos con filtros opcionales usando Dapper
        /// </summary>
        Task<IEnumerable<Vehicle>> GetAllDapperAsync(VehicleQueryFilter filters);

        /// <summary>
        /// Obtiene un vehículo por ID usando Dapper
        /// </summary>
        Task<Vehicle> GetByIdDapperAsync(int id);

        /// <summary>
        /// Obtiene un vehículo por su número de placa
        /// </summary>
        Task<Vehicle> GetByPlateNumberAsync(string plateNumber);

        /// <summary>
        /// Obtiene todos los vehículos activos
        /// </summary>
        Task<IEnumerable<Vehicle>> GetActiveVehiclesAsync();

        /// <summary>
        /// Obtiene vehículos disponibles para asignación
        /// </summary>
        Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();

        /// <summary>
        /// Obtiene vehículos por tipo
        /// </summary>
        Task<IEnumerable<Vehicle>> GetByTypeAsync(VehicleType type);

        /// <summary>
        /// Obtiene vehículos por estado
        /// </summary>
        Task<IEnumerable<Vehicle>> GetByStatusAsync(VehicleStatus status);

        /// <summary>
        /// Obtiene vehículos por almacén base
        /// </summary>
        Task<IEnumerable<Vehicle>> GetByWarehouseAsync(int warehouseId);

        /// <summary>
        /// Obtiene vehículos con capacidad suficiente
        /// </summary>
        Task<IEnumerable<Vehicle>> GetByCapacityAsync(double requiredWeight, double requiredVolume);

        /// <summary>
        /// Obtiene vehículos que requieren mantenimiento
        /// </summary>
        Task<IEnumerable<Vehicle>> GetVehiclesRequiringMaintenanceAsync();

        /// <summary>
        /// Obtiene estadísticas de un vehículo
        /// </summary>
        Task<VehicleStatisticsResponse> GetVehicleStatisticsAsync(int vehicleId);

        /// <summary>
        /// Verifica si un número de placa ya existe
        /// </summary>
        Task<bool> PlateNumberExistsAsync(string plateNumber, int? excludeVehicleId = null);

        /// <summary>
        /// Actualiza la carga actual de un vehículo
        /// </summary>
        Task UpdateCurrentLoadAsync(int vehicleId, double weight, double volume);

        /// <summary>
        /// Obtiene los vehículos más recientes
        /// </summary>
        Task<IEnumerable<Vehicle>> GetRecentVehiclesAsync(int limit = 10);
    }
}