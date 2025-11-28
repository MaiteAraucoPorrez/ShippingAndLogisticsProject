using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del repositorio para la entidad <see cref="Driver"/>.
    /// </summary>
    public interface IDriverRepository : IBaseRepository<Driver>
    {
        /// <summary>
        /// Obtiene todos los conductores con filtros opcionales usando Dapper
        /// </summary>
        Task<IEnumerable<Driver>> GetAllDapperAsync(DriverQueryFilter filters);

        /// <summary>
        /// Obtiene un conductor por ID usando Dapper
        /// </summary>
        Task<Driver> GetByIdDapperAsync(int id);

        /// <summary>
        /// Obtiene un conductor por su número de licencia
        /// </summary>
        Task<Driver> GetByLicenseNumberAsync(string licenseNumber);

        /// <summary>
        /// Obtiene un conductor por su documento de identidad
        /// </summary>
        Task<Driver> GetByIdentityDocumentAsync(string identityDocument);

        /// <summary>
        /// Obtiene todos los conductores activos
        /// </summary>
        Task<IEnumerable<Driver>> GetActiveDriversAsync();

        /// <summary>
        /// Obtiene conductores disponibles para asignación
        /// </summary>
        Task<IEnumerable<Driver>> GetAvailableDriversAsync();

        /// <summary>
        /// Obtiene conductores por estado
        /// </summary>
        Task<IEnumerable<Driver>> GetByStatusAsync(DriverStatus status);

        /// <summary>
        /// Obtiene conductores con licencia próxima a vencer
        /// </summary>
        Task<IEnumerable<Driver>> GetDriversWithExpiringLicensesAsync(int daysThreshold = 30);

        /// <summary>
        /// Obtiene estadísticas de un conductor
        /// </summary>
        Task<DriverStatisticsResponse> GetDriverStatisticsAsync(int driverId);

        /// <summary>
        /// Verifica si un número de licencia ya existe
        /// </summary>
        Task<bool> LicenseNumberExistsAsync(string licenseNumber, int? excludeDriverId = null);

        /// <summary>
        /// Verifica si un documento de identidad ya existe
        /// </summary>
        Task<bool> IdentityDocumentExistsAsync(string identityDocument, int? excludeDriverId = null);

        /// <summary>
        /// Obtiene los conductores más recientes
        /// </summary>
        Task<IEnumerable<Driver>> GetRecentDriversAsync(int limit = 10);
    }
}