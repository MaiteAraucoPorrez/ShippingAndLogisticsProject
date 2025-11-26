using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del repositorio para la entidad <see cref="Address"/>.
    /// Incluye métodos de consulta avanzada, filtrado y operaciones optimizadas con Dapper.
    /// </summary>
    public interface IAddressRepository : IBaseRepository<Address>
    {
        /// <summary>
        /// Obtiene todas las direcciones de un cliente específico
        /// </summary>
        /// <param name="customerId">ID del cliente</param>
        /// <returns>Lista de direcciones del cliente</returns>
        Task<IEnumerable<Address>> GetByCustomerIdAsync(int customerId);

        /// <summary>
        /// Obtiene todas las direcciones usando Dapper con filtros opcionales
        /// </summary>
        /// <param name="filters">Filtros de búsqueda</param>
        /// <returns>Lista filtrada de direcciones</returns>
        Task<IEnumerable<Address>> GetAllDapperAsync(AddressQueryFilter filters);

        /// <summary>
        /// Obtiene una dirección por ID usando Dapper
        /// </summary>
        /// <param name="id">ID de la dirección</param>
        /// <returns>Dirección encontrada o null</returns>
        Task<Address> GetByIdDapperAsync(int id);

        /// <summary>
        /// Obtiene la dirección predeterminada de un cliente según el tipo
        /// </summary>
        /// <param name="customerId">ID del cliente</param>
        /// <param name="type">Tipo de dirección (Pickup/Delivery)</param>
        /// <returns>Dirección predeterminada o null</returns>
        Task<Address> GetDefaultAddressAsync(int customerId, AddressType type);

        /// <summary>
        /// Obtiene todas las direcciones de una ciudad específica
        /// </summary>
        /// <param name="city">Nombre de la ciudad</param>
        /// <returns>Lista de direcciones en esa ciudad</returns>
        Task<IEnumerable<Address>> GetByCityAsync(string city);

        /// <summary>
        /// Obtiene todas las direcciones de un departamento
        /// </summary>
        /// <param name="department">Nombre del departamento</param>
        /// <returns>Lista de direcciones en ese departamento</returns>
        Task<IEnumerable<Address>> GetByDepartmentAsync(string department);

        /// <summary>
        /// Verifica si existe una dirección predeterminada para un cliente y tipo
        /// </summary>
        /// <param name="customerId">ID del cliente</param>
        /// <param name="type">Tipo de dirección</param>
        /// <param name="excludeAddressId">ID de dirección a excluir (para updates)</param>
        /// <returns>True si existe, False si no</returns>
        Task<bool> HasDefaultAddressAsync(int customerId, AddressType type, int? excludeAddressId = null);

        /// <summary>
        /// Desmarca todas las direcciones predeterminadas de un cliente para un tipo específico
        /// </summary>
        /// <param name="customerId">ID del cliente</param>
        /// <param name="type">Tipo de dirección</param>
        /// <param name="excludeAddressId">ID de dirección a excluir</param>
        Task UnsetDefaultAddressesAsync(int customerId, AddressType type, int? excludeAddressId = null);

        /// <summary>
        /// Cuenta las direcciones activas de un cliente
        /// </summary>
        /// <param name="customerId">ID del cliente</param>
        /// <returns>Cantidad de direcciones activas</returns>
        Task<int> CountActiveAddressesAsync(int customerId);

        /// <summary>
        /// Obtiene las direcciones más recientes del sistema
        /// </summary>
        /// <param name="limit">Cantidad de registros a retornar</param>
        /// <returns>Lista de direcciones recientes</returns>
        Task<IEnumerable<Address>> GetRecentAddressesAsync(int limit = 10);
    }
}