using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Enum;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ShippingAndLogisticsManagement.Core.QueryFilters
{
    /// <summary>
    /// Filtros de búsqueda para direcciones con soporte de paginación
    /// </summary>
    public class AddressQueryFilter : PaginationQueryFilter
    {
        /// <summary>
        /// Filtrar por ID de cliente
        /// </summary>
        /// <example>5</example>
        [SwaggerSchema("ID del cliente propietario", Nullable = true)]
        public int? CustomerId { get; set; }

        /// <summary>
        /// Filtrar por ciudad (búsqueda parcial)
        /// </summary>
        /// <example>La Paz</example>
        [SwaggerSchema("Buscar por ciudad", Nullable = true)]
        [MaxLength(100)]
        public string? City { get; set; }

        /// <summary>
        /// Filtrar por departamento
        /// </summary>
        /// <example>La Paz</example>
        [SwaggerSchema("Filtrar por departamento", Nullable = true)]
        [MaxLength(100)]
        public string? Department { get; set; }

        /// <summary>
        /// Filtrar por zona o barrio
        /// </summary>
        /// <example>Sopocachi</example>
        [SwaggerSchema("Buscar por zona", Nullable = true)]
        [MaxLength(100)]
        public string? Zone { get; set; }

        /// <summary>
        /// Filtrar por tipo de dirección
        /// </summary>
        /// <example>Delivery</example>
        [SwaggerSchema("Tipo de dirección: Pickup o Delivery", Nullable = true)]
        public AddressType? Type { get; set; }

        /// <summary>
        /// Filtrar solo direcciones predeterminadas
        /// </summary>
        /// <example>true</example>
        [SwaggerSchema("Mostrar solo direcciones predeterminadas", Nullable = true)]
        public bool? IsDefault { get; set; }

        /// <summary>
        /// Filtrar por estado activo/inactivo
        /// </summary>
        /// <example>true</example>
        [SwaggerSchema("Mostrar solo direcciones activas", Nullable = true)]
        public bool? IsActive { get; set; }

        /// <summary>
        /// Búsqueda por texto en calle o referencia
        /// </summary>
        /// <example>Av. 6 de Agosto</example>
        [SwaggerSchema("Buscar en dirección o referencia", Nullable = true)]
        [MaxLength(200)]
        public string? SearchText { get; set; }
    }
}