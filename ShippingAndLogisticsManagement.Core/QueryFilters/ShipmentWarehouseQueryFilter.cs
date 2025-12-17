using ShippingAndLogisticsManagement.Core.Enum;
using Swashbuckle.AspNetCore.Annotations;

namespace ShippingAndLogisticsManagement.Core.QueryFilters
{
    /// <summary>
    /// Filtros de búsqueda para movimientos de envíos en almacenes
    /// </summary>
    public class ShipmentWarehouseQueryFilter : PaginationQueryFilter
    {
        /// <summary>
        /// Filtrar por ID de envío
        /// </summary>
        [SwaggerSchema("ID del envío", Nullable = true)]
        public int? ShipmentId { get; set; }

        /// <summary>
        /// Filtrar por ID de almacén
        /// </summary>
        [SwaggerSchema("ID del almacén", Nullable = true)]
        public int? WarehouseId { get; set; }

        /// <summary>
        /// Filtrar por estado
        /// </summary>
        [SwaggerSchema("Estado: Received, InStorage, Processing, Dispatched", Nullable = true)]
        public WarehouseShipmentStatus? Status { get; set; }

        /// <summary>
        /// Filtrar por si tiene fecha de salida
        /// </summary>
        [SwaggerSchema("true = ya salió, false = aún en almacén", Nullable = true)]
        public bool? HasExited { get; set; }

        /// <summary>
        /// Fecha de entrada desde
        /// </summary>
        [SwaggerSchema("Fecha de entrada desde", Nullable = true)]
        public DateTime? EntryDateFrom { get; set; }

        /// <summary>
        /// Fecha de entrada hasta
        /// </summary>
        [SwaggerSchema("Fecha de entrada hasta", Nullable = true)]
        public DateTime? EntryDateTo { get; set; }
    }
}
