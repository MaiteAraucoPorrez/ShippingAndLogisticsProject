using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Enum;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ShippingAndLogisticsManagement.Core.QueryFilters
{
    /// <summary>
    /// Filtros de búsqueda para conductores con soporte de paginación
    /// </summary>
    public class DriverQueryFilter : PaginationQueryFilter
    {
        /// <summary>
        /// Filtrar por nombre completo (búsqueda parcial)
        /// </summary>
        [SwaggerSchema("Buscar por nombre del conductor", Nullable = true)]
        [MaxLength(100)]
        public string? FullName { get; set; }

        /// <summary>
        /// Filtrar por número de licencia
        /// </summary>
        [SwaggerSchema("Buscar por número de licencia", Nullable = true)]
        [MaxLength(50)]
        public string? LicenseNumber { get; set; }

        /// <summary>
        /// Filtrar por categoría de licencia
        /// </summary>
        [SwaggerSchema("Filtrar por categoría (A, B, C)", Nullable = true)]
        [MaxLength(20)]
        public string? LicenseCategory { get; set; }

        /// <summary>
        /// Filtrar por estado del conductor
        /// </summary>
        [SwaggerSchema("Estado: Available, OnRoute, OffDuty, OnLeave", Nullable = true)]
        public DriverStatus? Status { get; set; }

        /// <summary>
        /// Filtrar por estado activo/inactivo
        /// </summary>
        [SwaggerSchema("Mostrar solo conductores activos", Nullable = true)]
        public bool? IsActive { get; set; }

        /// <summary>
        /// Filtrar por vehículo asignado
        /// </summary>
        [SwaggerSchema("ID del vehículo asignado", Nullable = true)]
        public int? CurrentVehicleId { get; set; }

        /// <summary>
        /// Filtrar conductores con licencia próxima a vencer
        /// </summary>
        [SwaggerSchema("Días hasta vencimiento de licencia (ej: 30)", Nullable = true)]
        public int? LicenseExpiringInDays { get; set; }

        /// <summary>
        /// Filtrar por años mínimos de experiencia
        /// </summary>
        [SwaggerSchema("Años mínimos de experiencia", Nullable = true)]
        public int? MinYearsOfExperience { get; set; }

        /// <summary>
        /// Filtrar por calificación mínima
        /// </summary>
        [SwaggerSchema("Calificación mínima (1-5)", Nullable = true)]
        [Range(1, 5)]
        public double? MinAverageRating { get; set; }

        /// <summary>
        /// Búsqueda por email
        /// </summary>
        [SwaggerSchema("Buscar por email", Nullable = true)]
        [MaxLength(100)]
        public string? Email { get; set; }

        /// <summary>
        /// Búsqueda por teléfono
        /// </summary>
        [SwaggerSchema("Buscar por teléfono", Nullable = true)]
        [MaxLength(20)]
        public string? Phone { get; set; }
    }
}