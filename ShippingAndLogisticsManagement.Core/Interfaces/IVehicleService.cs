using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del servicio para la gestión de vehículos.
    /// Permite realizar operaciones CRUD, consultas filtradas y estadísticas.
    /// </summary>
    public interface IVehicleService
    {
        /// <summary>
        /// Obtiene una lista paginada y filtrada de vehículos
        /// </summary>
        Task<ResponseData> GetAllAsync(VehicleQueryFilter filters);

        /// <summary>
        /// Obtiene todos los vehículos usando Dapper
        /// </summary>
        Task<IEnumerable<Vehicle>> GetAllDapperAsync();

        /// <summary>
        /// Obtiene un vehículo por ID
        /// </summary>
        Task<Vehicle> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene un vehículo por ID usando Dapper
        /// </summary>
        Task<Vehicle> GetByIdDapperAsync(int id);

        /// <summary>
        /// Obtiene vehículos disponibles para asignación
        /// </summary>
        Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();

        /// <summary>
        /// Obtiene vehículos que requieren mantenimiento
        /// </summary>
        Task<IEnumerable<Vehicle>> GetVehiclesRequiringMaintenanceAsync();

        /// <summary>
        /// Obtiene vehículos por capacidad requerida
        /// </summary>
        Task<IEnumerable<Vehicle>> GetByCapacityAsync(double requiredWeight, double requiredVolume);

        /// <summary>
        /// Obtiene estadísticas de un vehículo
        /// </summary>
        Task<VehicleStatisticsResponse> GetVehicleStatisticsAsync(int vehicleId);

        /// <summary>
        /// Crea un nuevo vehículo
        /// </summary>
        Task InsertAsync(Vehicle vehicle);

        /// <summary>
        /// Actualiza un vehículo existente
        /// </summary>
        Task UpdateAsync(Vehicle vehicle);

        /// <summary>
        /// Elimina un vehículo
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Desactiva un vehículo sin eliminarlo
        /// </summary>
        Task DeactivateAsync(int id);

        /// <summary>
        /// Actualiza la carga actual de un vehículo
        /// </summary>
        Task UpdateCurrentLoadAsync(int vehicleId, double weight, double volume);

        /// <summary>
        /// Asigna un conductor a un vehículo
        /// </summary>
        Task AssignDriverAsync(int vehicleId, int driverId);

        /// <summary>
        /// Remueve el conductor asignado a un vehículo
        /// </summary>
        Task UnassignDriverAsync(int vehicleId);
    }
}