using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del repositorio para la entidad <see cref="Shipment"/>.
    /// Permite acceder y manipular los datos de envíos mediante consultas directas y optimizadas con Dapper.
    /// </summary>
    public interface IShipmentRepository : IBaseRepository<Shipment>
    {
        /// <summary>
        /// Obtiene todos los envíos asociados a un cliente específico.
        /// </summary>
        /// <param name="customerId">Identificador único del cliente.</param>
        /// <returns>Colección de envíos pertenecientes al cliente indicado.</returns>
        Task<IEnumerable<Shipment>> GetAllAsync(int customerId);

        /// <summary>
        /// Obtiene una lista de los envíos más recientes, limitada por un número especificado.
        /// </summary>
        /// <param name="limit">Número máximo de registros a retornar (por defecto 10).</param>
        /// <returns>Colección de los envíos más recientes ordenados por fecha descendente.</returns>
        Task<IEnumerable<Shipment>> GetRecentShipmentsAsync(int limit = 10);

        /// <summary>
        /// Obtiene todos los envíos usando Dapper aplicando filtros de búsqueda.
        /// </summary>
        /// <param name="filters">Filtros de búsqueda que pueden incluir fecha, estado, cliente, etc.</param>
        /// <returns>Colección filtrada de envíos obtenidos mediante Dapper.</returns>
        Task<IEnumerable<Shipment>> GetAllDapperAsync(ShipmentQueryFilter filters);

        /// <summary>
        /// Obtiene un envío específico por su identificador utilizando Dapper.
        /// </summary>
        /// <param name="id">Identificador único del envío.</param>
        /// <returns>El envío correspondiente al ID especificado.</returns>
        Task<Shipment> GetByIdDapperAsync(int id);

        /// <summary>
        /// Obtiene información combinada de envíos, clientes y rutas mediante Dapper.
        /// </summary>
        /// <returns>Colección de objetos con los detalles del envío, cliente y ruta asociados.</returns>
        Task<IEnumerable<ShipmentCustomerRouteResponse>> GetShipmentCustomerRouteAsync();

        // Métodos CRUD básicos heredados de IBaseRepository:
        // Task<Shipment> GetByIdAsync(int id);
        // Task InsertAsync(Shipment shipment);
        // Task UpdateAsync(Shipment shipment);
        // Task DeleteAsync(Shipment shipment);
    }
}

