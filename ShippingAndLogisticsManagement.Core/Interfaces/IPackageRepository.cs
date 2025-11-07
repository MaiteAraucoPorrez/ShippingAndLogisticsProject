using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del repositorio para la entidad <see cref="Package"/>.
    /// Incluye métodos de consulta avanzada, filtrado, estadísticas y operaciones optimizadas con Dapper.
    /// </summary>
    public interface IPackageRepository : IBaseRepository<Package>
    {
        /// <summary>
        /// Obtiene todos los paquetes asociados a un cliente específico.
        /// </summary>
        /// <param name="CustomerId">Identificador único del cliente.</param>
        /// <returns>Colección de paquetes correspondientes al cliente indicado.</returns>
        Task<IEnumerable<Package>> GetAllAsync(int CustomerId);

        /// <summary>
        /// Obtiene una lista de los paquetes más recientes, limitada por un número especificado.
        /// </summary>
        /// <param name="limit">Número máximo de registros a retornar (por defecto 10).</param>
        /// <returns>Colección de los paquetes más recientes ordenados por fecha descendente.</returns>
        Task<IEnumerable<Package>> GetRecentPackagesAsync(int limit = 10);

        /// <summary>
        /// Obtiene todos los paquetes usando Dapper aplicando filtros de búsqueda.
        /// </summary>
        /// <param name="filters">Filtros de búsqueda que pueden incluir peso, estado o fecha de envío.</param>
        /// <returns>Colección de paquetes filtrados obtenidos mediante Dapper.</returns>
        Task<IEnumerable<Package>> GetAllDapperAsync(PackageQueryFilter filters);

        /// <summary>
        /// Obtiene un paquete específico por su identificador utilizando Dapper.
        /// </summary>
        /// <param name="id">Identificador único del paquete.</param>
        /// <returns>El paquete correspondiente al ID especificado.</returns>
        Task<Package> GetByIdDapperAsync(int id);

        /// <summary>
        /// Obtiene todos los paquetes pertenecientes a un envío específico mediante Dapper.
        /// </summary>
        /// <param name="shipmentId">Identificador único del envío.</param>
        /// <returns>Colección de paquetes asociados al envío indicado.</returns>
        Task<IEnumerable<Package>> GetByShipmentIdDapperAsync(int shipmentId);

        /// <summary>
        /// Obtiene un resumen estadístico de los paquetes de un envío específico.
        /// Incluye datos como el total de paquetes, peso total, valor total y promedios.
        /// </summary>
        /// <param name="shipmentId">Identificador único del envío.</param>
        /// <returns>Objeto con información estadística de los paquetes.</returns>
        Task<PackageSummaryResponse> GetPackageSummaryAsync(int shipmentId);

        /// <summary>
        /// Obtiene paquetes con información completa, incluyendo datos del envío, cliente y ruta (JOIN).
        /// </summary>
        /// <returns>Colección con detalles completos de cada paquete y sus relaciones asociadas.</returns>
        Task<IEnumerable<PackageDetailResponse>> GetPackageWithDetailsAsync();

        /// <summary>
        /// Obtiene los paquetes cuyo peso supera un valor mínimo especificado.
        /// </summary>
        /// <param name="minWeight">Peso mínimo en kilogramos.</param>
        /// <returns>Colección de paquetes ordenados por peso descendente.</returns>
        Task<IEnumerable<Package>> GetHeavyPackagesAsync(double minWeight);
    }
}
