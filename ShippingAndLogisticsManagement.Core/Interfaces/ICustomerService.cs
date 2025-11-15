using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del servicio para la gestión de clientes (Customers).
    /// Permite realizar operaciones CRUD, consultas filtradas, historial de envíos y validaciones de negocio.
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>
        /// Obtiene una lista paginada y filtrada de clientes según los parámetros del filtro.
        /// </summary>
        /// <param name="customerQueryFilter">Filtro de búsqueda con criterios como nombre, email o teléfono.</param>
        /// <returns>Una respuesta que contiene la lista de clientes y la información de paginación.</returns>
        Task<ResponseData> GetAllAsync(CustomerQueryFilter customerQueryFilter);

        /// <summary>
        /// Obtiene todos los clientes usando Dapper para mejorar el rendimiento de la consulta.
        /// </summary>
        /// <returns>Colección de clientes obtenidos directamente desde la base de datos.</returns>
        Task<IEnumerable<Customer>> GetAllDapperAsync();

        /// <summary>
        /// Obtiene el historial completo de envíos de un cliente específico.
        /// </summary>
        /// <param name="customerId">Identificador único del cliente.</param>
        /// <returns>Lista con información detallada de todos los envíos del cliente.</returns>
        Task<IEnumerable<CustomerShipmentHistoryResponse>> GetCustomerShipmentHistoryAsync(int customerId);

        /// <summary>
        /// Obtiene un cliente específico por su identificador (ID).
        /// </summary>
        /// <param name="id">Identificador único del cliente.</param>
        /// <returns>El cliente correspondiente al ID especificado.</returns>
        Task<Customer> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene un cliente por ID utilizando Dapper (consulta optimizada).
        /// </summary>
        /// <param name="id">Identificador único del cliente.</param>
        /// <returns>El cliente correspondiente al ID especificado.</returns>
        Task<Customer> GetByIdDapperAsync(int id);

        /// <summary>
        /// Inserta un nuevo registro de cliente en la base de datos.
        /// </summary>
        /// <param name="customer">Objeto de tipo Customer con los datos del nuevo cliente.</param>
        Task InsertAsync(Customer customer);

        /// <summary>
        /// Actualiza los datos de un cliente existente.
        /// </summary>
        /// <param name="customer">Objeto de tipo Customer con la información actualizada.</param>
        Task UpdateAsync(Customer customer);

        /// <summary>
        /// Elimina un cliente por su identificador (ID).
        /// </summary>
        /// <param name="id">Identificador único del cliente a eliminar.</param>
        Task DeleteAsync(int id);
    }
}
