using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del servicio para la gestión de conductores.
    /// Permite realizar operaciones CRUD, consultas filtradas y estadísticas.
    /// </summary>
    public interface IDriverService
    {
        /// <summary>
        /// Obtiene una lista paginada y filtrada de conductores
        /// </summary>
        Task<ResponseData> GetAllAsync(DriverQueryFilter filters);

        /// <summary>
        /// Obtiene todos los conductores usando Dapper
        /// </summary>
        Task<IEnumerable<Driver>> GetAllDapperAsync();

        /// <summary>
        /// Obtiene un conductor por ID
        /// </summary>
        Task<Driver> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene un conductor por ID usando Dapper
        /// </summary>
        Task<Driver> GetByIdDapperAsync(int id);

        /// <summary>
        /// Obtiene conductores disponibles para asignación
        /// </summary>
        Task<IEnumerable<Driver>> GetAvailableDriversAsync();

        /// <summary>
        /// Obtiene estadísticas de un conductor
        /// </summary>
        Task<DriverStatisticsResponse> GetDriverStatisticsAsync(int driverId);

        /// <summary>
        /// Crea un nuevo conductor
        /// </summary>
        Task InsertAsync(Driver driver);

        /// <summary>
        /// Actualiza un conductor existente
        /// </summary>
        Task UpdateAsync(Driver driver);

        /// <summary>
        /// Elimina un conductor
        /// </summary>
        Task DeleteAsync(int id);
    }
}