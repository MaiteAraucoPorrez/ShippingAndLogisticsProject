using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Enum;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del servicio para la gestión de direcciones.
    /// Permite realizar operaciones CRUD, consultas filtradas y validaciones de negocio.
    /// </summary>
    public interface IAddressService
    {
        /// <summary>
        /// Obtiene una lista paginada y filtrada de direcciones
        /// </summary>
        /// <param name="filters">Filtros de búsqueda</param>
        /// <returns>Respuesta con lista paginada de direcciones</returns>
        Task<ResponseData> GetAllAsync(AddressQueryFilter filters);

        /// <summary>
        /// Obtiene todas las direcciones de un cliente
        /// </summary>
        /// <param name="customerId">ID del cliente</param>
        /// <returns>Lista de direcciones del cliente</returns>
        Task<IEnumerable<Address>> GetByCustomerIdAsync(int customerId);

        /// <summary>
        /// Obtiene todas las direcciones usando Dapper
        /// </summary>
        /// <returns>Lista de direcciones</returns>
        Task<IEnumerable<Address>> GetAllDapperAsync();

        /// <summary>
        /// Obtiene una dirección por ID
        /// </summary>
        /// <param name="id">ID de la dirección</param>
        /// <returns>Dirección encontrada</returns>
        Task<Address> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene una dirección por ID usando Dapper
        /// </summary>
        /// <param name="id">ID de la dirección</param>
        /// <returns>Dirección encontrada</returns>
        Task<Address> GetByIdDapperAsync(int id);

        /// <summary>
        /// Obtiene la dirección predeterminada de un cliente
        /// </summary>
        /// <param name="customerId">ID del cliente</param>
        /// <param name="type">Tipo de dirección</param>
        /// <returns>Dirección predeterminada o null</returns>
        Task<Address> GetDefaultAddressAsync(int customerId, AddressType type);

        /// <summary>
        /// Crea una nueva dirección
        /// </summary>
        /// <param name="address">Datos de la dirección</param>
        Task InsertAsync(Address address);

        /// <summary>
        /// Actualiza una dirección existente
        /// </summary>
        /// <param name="address">Datos actualizados</param>
        Task UpdateAsync(Address address);

        /// <summary>
        /// Elimina una dirección
        /// </summary>
        /// <param name="id">ID de la dirección a eliminar</param>
        Task DeleteAsync(int id);

        /// <summary>
        /// Desactiva una dirección sin eliminarla
        /// </summary>
        /// <param name="id">ID de la dirección</param>
        Task DeactivateAsync(int id);

        /// <summary>
        /// Marca una dirección como predeterminada
        /// </summary>
        /// <param name="id">ID de la dirección</param>
        Task SetAsDefaultAsync(int id);
    }
}