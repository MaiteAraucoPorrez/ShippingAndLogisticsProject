using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del repositorio para la entidad <see cref="Route"/>.
    /// Incluye métodos de consulta avanzada, filtrado, ranking y operaciones optimizadas con Dapper.
    /// </summary>
    public interface IRouteRepository : IBaseRepository<Route>
    {
        /// <summary>
        /// Obtiene todas las rutas usando Dapper aplicando filtros de búsqueda.
        /// </summary>
        /// <param name="filters">Filtros de búsqueda que pueden incluir origen, destino o estado.</param>
        /// <returns>Colección filtrada de rutas obtenidas mediante Dapper.</returns>
        Task<IEnumerable<Route>> GetAllDapperAsync(RouteQueryFilter filters);

        /// <summary>
        /// Obtiene una ruta específica por su identificador utilizando Dapper.
        /// </summary>
        /// <param name="id">Identificador único de la ruta.</param>
        /// <returns>La ruta correspondiente al ID especificado.</returns>
        Task<Route> GetByIdDapperAsync(int id);

        /// <summary>
        /// Obtiene solo las rutas activas disponibles para nuevos envíos.
        /// </summary>
        /// <returns>Colección de rutas marcadas como activas (IsActive = true).</returns>
        Task<IEnumerable<Route>> GetActiveRoutesAsync();

        /// <summary>
        /// Obtiene el ranking de las rutas más utilizadas basado en cantidad de envíos.
        /// </summary>
        /// <param name="limit">Cantidad de rutas a retornar en el ranking (por defecto 10).</param>
        /// <returns>Lista ordenada con estadísticas de uso de cada ruta.</returns>
        Task<IEnumerable<RouteRankingResponse>> GetMostUsedRoutesAsync(int limit = 10);

        /// <summary>
        /// Busca una ruta por origen y destino exactos.
        /// </summary>
        /// <param name="origin">Ciudad o ubicación de origen.</param>
        /// <param name="destination">Ciudad o ubicación de destino.</param>
        /// <returns>La ruta que coincide con origen y destino, o null si no existe.</returns>
        Task<Route> GetByOriginDestinationAsync(string origin, string destination);

        /// <summary>
        /// Verifica si una ruta tiene envíos asociados.
        /// </summary>
        /// <param name="routeId">Identificador único de la ruta.</param>
        /// <returns>True si tiene envíos asociados, False en caso contrario.</returns>
        Task<bool> HasShipmentsAsync(int routeId);

        /// <summary>
        /// Obtiene las rutas más recientes, limitadas por un número especificado.
        /// </summary>
        /// <param name="limit">Número máximo de registros a retornar (por defecto 10).</param>
        /// <returns>Colección de las rutas más recientes ordenadas por ID descendente.</returns>
        Task<IEnumerable<Route>> GetRecentRoutesAsync(int limit = 10);
    }
}
