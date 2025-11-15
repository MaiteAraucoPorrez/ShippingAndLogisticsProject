using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del servicio para la gestión de rutas (Routes).
    /// Permite realizar operaciones CRUD, consultas filtradas, ranking de rutas y validaciones de negocio.
    /// </summary>
    public interface IRouteService
    {
        /// <summary>
        /// Obtiene una lista paginada y filtrada de rutas según los parámetros del filtro.
        /// </summary>
        /// <param name="routeQueryFilter">Filtro de búsqueda con criterios como origen, destino o estado activo.</param>
        /// <returns>Una respuesta que contiene la lista de rutas y la información de paginación.</returns>
        Task<ResponseData> GetAllAsync(RouteQueryFilter routeQueryFilter);

        /// <summary>
        /// Obtiene todas las rutas usando Dapper para mejorar el rendimiento de la consulta.
        /// </summary>
        /// <returns>Colección de rutas obtenidas directamente desde la base de datos.</returns>
        Task<IEnumerable<Route>> GetAllDapperAsync();

        /// <summary>
        /// Obtiene solo las rutas activas disponibles para nuevos envíos.
        /// </summary>
        /// <returns>Lista de rutas marcadas como activas (IsActive = true).</returns>
        Task<IEnumerable<Route>> GetActiveRoutesAsync();

        /// <summary>
        /// Obtiene el ranking de las rutas más utilizadas basado en cantidad de envíos.
        /// </summary>
        /// <param name="limit">Cantidad de rutas a retornar en el ranking.</param>
        /// <returns>Lista ordenada de rutas con sus estadísticas de uso.</returns>
        Task<IEnumerable<RouteRankingResponse>> GetMostUsedRoutesAsync(int limit = 10);

        /// <summary>
        /// Obtiene una ruta específica por su identificador (ID).
        /// </summary>
        /// <param name="id">Identificador único de la ruta.</param>
        /// <returns>La ruta correspondiente al ID especificado.</returns>
        Task<Route> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene una ruta por ID utilizando Dapper (consulta optimizada).
        /// </summary>
        /// <param name="id">Identificador único de la ruta.</param>
        /// <returns>La ruta correspondiente al ID especificado.</returns>
        Task<Route> GetByIdDapperAsync(int id);

        /// <summary>
        /// Inserta un nuevo registro de ruta en la base de datos.
        /// </summary>
        /// <param name="route">Objeto de tipo Route con los datos de la nueva ruta.</param>
        Task InsertAsync(Route route);

        /// <summary>
        /// Actualiza los datos de una ruta existente.
        /// </summary>
        /// <param name="route">Objeto de tipo Route con la información actualizada.</param>
        Task UpdateAsync(Route route);

        /// <summary>
        /// Elimina una ruta por su identificador (ID).
        /// </summary>
        /// <param name="id">Identificador único de la ruta a eliminar.</param>
        Task DeleteAsync(int id);
    }
}
