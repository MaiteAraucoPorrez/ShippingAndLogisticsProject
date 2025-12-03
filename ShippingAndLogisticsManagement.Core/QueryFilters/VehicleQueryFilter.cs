using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Enum;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ShippingAndLogisticsManagement.Core.QueryFilters
{
    /// <summary>
    /// Filtros de búsqueda para vehículos con soporte de paginación
    /// </summary>
    public class VehicleQueryFilter : PaginationQueryFilter
    {
        /// <summary>
        /// Filtrar por número de placa
        /// </summary>
        [SwaggerSchema("Buscar por placa", Nullable = true)]
        [MaxLength(20)]
        public string? PlateNumber { get; set; }

        /// <summary>
        /// Filtrar por tipo de vehículo
        /// </summary>
        [SwaggerSchema("Tipo: Motorcycle, Van, Pickup, Truck", Nullable = true)]
        public VehicleType? Type { get; set; }

        /// <summary>
        /// Filtrar por estado
        /// </summary>
        [SwaggerSchema("Estado: Available, InTransit, Maintenance, OutOfService", Nullable = true)]
        public VehicleStatus? Status { get; set; }

        /// <summary>
        /// Filtrar por estado activo/inactivo
        /// </summary>
        [SwaggerSchema("Mostrar solo vehículos activos", Nullable = true)]
        public bool? IsActive { get; set; }

        /// <summary>
        /// Filtrar por almacén base
        /// </summary>
        [SwaggerSchema("ID del almacén base", Nullable = true)]
        public int? BaseWarehouseId { get; set; }

        /// <summary>
        /// Filtrar por conductor asignado
        /// </summary>
        [SwaggerSchema("ID del conductor asignado", Nullable = true)]
        public int? AssignedDriverId { get; set; }

        /// <summary>
        /// Filtrar vehículos con capacidad de peso mínima
        /// </summary>
        [SwaggerSchema("Capacidad de peso disponible mínima en kg", Nullable = true)]
        public double? MinAvailableWeightKg { get; set; }

        /// <summary>
        /// Filtrar vehículos con capacidad de volumen mínima
        /// </summary>
        [SwaggerSchema("Capacidad de volumen disponible mínima en m³", Nullable = true)]
        public double? MinAvailableVolumeM3 { get; set; }

        /// <summary>
        /// Filtrar vehículos que requieren mantenimiento
        /// </summary>
        [SwaggerSchema("Mostrar solo vehículos que requieren mantenimiento", Nullable = true)]
        public bool? RequiresMaintenance { get; set; }
    }
}