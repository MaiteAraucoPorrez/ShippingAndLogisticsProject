using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del servicio para la gestión de envíos (Shipments).
    /// Permite realizar operaciones CRUD, consultas personalizadas y consultas optimizadas con Dapper.
    /// </summary>
    public interface IShipmentService
    {
        /// <summary>
        /// Obtiene una lista paginada y filtrada de envíos según los parámetros del filtro.
        /// </summary>
        /// <param name="shipmentQueryFilter">Filtro de búsqueda con parámetros como fechas, estado o cliente.</param>
        /// <returns>Una respuesta que contiene la lista de envíos y la información de paginación.</returns>
        Task<ResponseData> GetAllAsync(ShipmentQueryFilter shipmentQueryFilter);

        /// <summary>
        /// Obtiene todos los envíos usando Dapper, optimizando el rendimiento de la consulta.
        /// </summary>
        /// <returns>Colección de envíos obtenidos directamente desde la base de datos.</returns>
        Task<IEnumerable<Shipment>> GetAllDapperAsync();

        /// <summary>
        /// Obtiene una lista combinada de envíos, clientes y rutas utilizando Dapper.
        /// </summary>
        /// <returns>Colección con información detallada de cada envío, cliente y su respectiva ruta.</returns>
        Task<IEnumerable<ShipmentCustomerRouteResponse>> GetShipmentCustomerRouteAsync();

        /// <summary>
        /// Obtiene un envío específico por su identificador (ID).
        /// </summary>
        /// <param name="id">Identificador único del envío.</param>
        /// <returns>El envío correspondiente al ID especificado.</returns>
        Task<Shipment> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene un envío por su ID utilizando Dapper (consulta optimizada).
        /// </summary>
        /// <param name="id">Identificador único del envío.</param>
        /// <returns>El envío correspondiente al ID especificado.</returns>
        Task<Shipment> GetByIdDapperAsync(int id);

        /// <summary>
        /// Inserta un nuevo registro de envío en la base de datos.
        /// </summary>
        /// <param name="shipment">Objeto de tipo Shipment con los datos del nuevo envío.</param>
        Task InsertAsync(Shipment shipment);

        /// <summary>
        /// Actualiza los datos de un envío existente.
        /// </summary>
        /// <param name="shipment">Objeto de tipo Shipment con la información actualizada.</param>
        Task UpdateAsync(Shipment shipment);

        /// <summary>
        /// Elimina un envío por su identificador (ID).
        /// </summary>
        /// <param name="id">Identificador único del envío a eliminar.</param>
        Task DeleteAsync(int id);
    }
}
