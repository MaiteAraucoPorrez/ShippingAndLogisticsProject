using ShippingAndLogisticsManagement.Core.Enum;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ShippingAndLogisticsManagement.Core.QueryFilters
{
    /// <summary>
    /// Filtros de búsqueda para almacenes con soporte de paginación
    /// </summary>
    public class WarehouseQueryFilter : PaginationQueryFilter
    {
        /// <summary>
        /// Filtrar por nombre (búsqueda parcial)
        /// </summary>
        [SwaggerSchema("Buscar por nombre del almacén", Nullable = true)]
        [MaxLength(100)]
        public string? Name { get; set; }

        /// <summary>
        /// Filtrar por código
        /// </summary>
        [SwaggerSchema("Buscar por código", Nullable = true)]
        [MaxLength(20)]
        public string? Code { get; set; }

        /// <summary>
        /// Filtrar por ciudad
        /// </summary>
        [SwaggerSchema("Filtrar por ciudad", Nullable = true)]
        [MaxLength(100)]
        public string? City { get; set; }

        /// <summary>
        /// Filtrar por departamento
        /// </summary>
        [SwaggerSchema("Filtrar por departamento", Nullable = true)]
        [MaxLength(100)]
        public string? Department { get; set; }

        /// <summary>
        /// Filtrar por tipo de almacén
        /// </summary>
        [SwaggerSchema("Tipo: Central, Regional o Local", Nullable = true)]
        public WarehouseType? Type { get; set; }

        /// <summary>
        /// Filtrar por estado activo/inactivo
        /// </summary>
        [SwaggerSchema("Mostrar solo almacenes activos", Nullable = true)]
        public bool? IsActive { get; set; }

        /// <summary>
        /// Filtrar almacenes con capacidad disponible mínima
        /// </summary>
        [SwaggerSchema("Capacidad disponible mínima en m3", Nullable = true)]
        public double? MinAvailableCapacity { get; set; }

        /// <summary>
        /// Filtrar por porcentaje máximo de ocupación
        /// </summary>
        [SwaggerSchema("Porcentaje máximo de ocupación (0-100)", Nullable = true)]
        [Range(0, 100)]
        public double? MaxOccupancyPercentage { get; set; }
    }
}
