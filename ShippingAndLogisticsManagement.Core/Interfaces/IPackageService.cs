using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del servicio para la gestión de paquetes (Packages).
    /// Permite realizar operaciones CRUD, consultas filtradas, resúmenes y consultas optimizadas con Dapper.
    /// </summary>
    public interface IPackageService
    {
        /// <summary>
        /// Obtiene una lista paginada y filtrada de paquetes según los parámetros del filtro.
        /// </summary>
        /// <param name="packageQueryFilter">Filtro de búsqueda con criterios como peso, estado o fecha de envío.</param>
        /// <returns>Una respuesta que contiene la lista de paquetes y la información de paginación.</returns>
        Task<ResponseData> GetAllAsync(PackageQueryFilter packageQueryFilter);

        /// <summary>
        /// Obtiene todos los paquetes usando Dapper para mejorar el rendimiento de la consulta.
        /// </summary>
        /// <returns>Colección de paquetes obtenidos directamente desde la base de datos.</returns>
        Task<IEnumerable<Package>> GetAllDapperAsync();

        /// <summary>
        /// Obtiene un resumen de los paquetes asociados a un envío específico.
        /// </summary>
        /// <param name="shipmentId">Identificador único del envío.</param>
        /// <returns>Un objeto con información agregada, como el número total de paquetes y el peso total.</returns>
        Task<PackageSummaryResponse> GetPackageSummaryAsync(int shipmentId);

        /// <summary>
        /// Obtiene información detallada de paquetes con datos de envío, cliente y ruta
        /// </summary>
        Task<IEnumerable<PackageDetailResponse>> GetPackageDetailsAsync();

        /// <summary>
        /// Obtiene los paquetes cuyo peso supera un valor mínimo especificado.
        /// </summary>
        /// <param name="minWeight">Peso mínimo en kilogramos.</param>
        /// <returns>Colección de paquetes ordenados por peso en orden descendente.</returns>
        Task<IEnumerable<Package>> GetHeavyPackagesAsync(double minWeight);

        /// <summary>
        /// Obtiene un paquete específico por su identificador (ID).
        /// </summary>
        /// <param name="id">Identificador único del paquete.</param>
        /// <returns>El paquete correspondiente al ID especificado.</returns>
        Task<Package> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene un paquete por ID utilizando Dapper (consulta optimizada).
        /// </summary>
        /// <param name="id">Identificador único del paquete.</param>
        /// <returns>El paquete correspondiente al ID especificado.</returns>
        Task<Package> GetByIdDapperAsync(int id);

        /// <summary>
        /// Inserta un nuevo registro de paquete en la base de datos.
        /// </summary>
        /// <param name="package">Objeto de tipo Package con los datos del nuevo paquete.</param>
        Task InsertAsync(Package package);

        /// <summary>
        /// Actualiza los datos de un paquete existente.
        /// </summary>
        /// <param name="package">Objeto de tipo Package con la información actualizada.</param>
        Task UpdateAsync(Package package);

        /// <summary>
        /// Elimina un paquete por su identificador (ID).
        /// </summary>
        /// <param name="id">Identificador único del paquete a eliminar.</param>
        Task DeleteAsync(int id);
    }
}

