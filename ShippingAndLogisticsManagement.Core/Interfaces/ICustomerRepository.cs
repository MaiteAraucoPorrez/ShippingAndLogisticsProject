using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    // <summary>
    /// Define las operaciones del repositorio para la entidad <see cref="Customer"/>.
    /// Incluye métodos de consulta avanzada, filtrado, historial y operaciones optimizadas con Dapper.
    /// </summary>
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        /// <summary>
        /// Obtiene todos los clientes usando Dapper aplicando filtros de búsqueda.
        /// </summary>
        /// <param name="filters">Filtros de búsqueda que pueden incluir nombre, email o teléfono.</param>
        /// <returns>Colección filtrada de clientes obtenidos mediante Dapper.</returns>
        Task<IEnumerable<Customer>> GetAllDapperAsync(CustomerQueryFilter filters);

        /// <summary>
        /// Obtiene un cliente específico por su identificador utilizando Dapper.
        /// </summary>
        /// <param name="id">Identificador único del cliente.</param>
        /// <returns>El cliente correspondiente al ID especificado.</returns>
        Task<Customer> GetByIdDapperAsync(int id);

        /// <summary>
        /// Obtiene un cliente por su dirección de correo electrónico.
        /// </summary>
        /// <param name="email">Correo electrónico del cliente.</param>
        /// <returns>El cliente con el email especificado, o null si no existe.</returns>
        Task<Customer> GetByEmailAsync(string email);

        /// <summary>
        /// Obtiene el historial completo de envíos de un cliente con información detallada.
        /// Incluye datos del envío, paquetes, rutas y estados.
        /// </summary>
        /// <param name="customerId">Identificador único del cliente.</param>
        /// <returns>Lista con información detallada de todos los envíos del cliente.</returns>
        Task<IEnumerable<CustomerShipmentHistoryResponse>> GetCustomerShipmentHistoryAsync(int customerId);

        /// <summary>
        /// Cuenta cuántos clientes existen con un dominio de email específico.
        /// </summary>
        /// <param name="emailDomain">Dominio del email (ej: gmail.com).</param>
        /// <returns>Cantidad de clientes con ese dominio de email.</returns>
        Task<int> CountByEmailDomainAsync(string emailDomain);

        /// <summary>
        /// Verifica si un cliente tiene envíos activos (no entregados).
        /// </summary>
        /// <param name="customerId">Identificador único del cliente.</param>
        /// <returns>True si tiene envíos activos, False en caso contrario.</returns>
        Task<bool> HasActiveShipmentsAsync(int customerId);

        /// <summary>
        /// Obtiene los clientes más recientes, limitados por un número especificado.
        /// </summary>
        /// <param name="limit">Número máximo de registros a retornar (por defecto 10).</param>
        /// <returns>Colección de los clientes más recientes ordenados por ID descendente.</returns>
        Task<IEnumerable<Customer>> GetRecentCustomersAsync(int limit = 10);
    }
}
