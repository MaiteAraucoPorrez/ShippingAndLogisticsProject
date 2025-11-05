using Swashbuckle.AspNetCore.Annotations;

namespace ShippingAndLogisticsManagement.Core.QueryFilters
{
    /// <summary>
    /// Represents query filters for packages with pagination support
    /// </summary>
    /// <remarks>
    /// Allows filtering packages by multiple criteria including shipment ID,
    /// weight range, price range, and description. All filters are optional.
    /// </remarks>
    public class PackageQueryFilter : PaginationQueryFilter
    {
        /// <summary>
        /// Shipment ID to which the package belongs
        /// </summary>
        [SwaggerSchema("Filtrar por ID de envio", Nullable = true)]
        public int? ShipmentId { get; set; }

        /// <summary>
        /// Description or part of the package description
        /// </summary>
        [SwaggerSchema("Buscar por descripcion del paquete", Nullable = true)]
        public string? Description { get; set; }

        /// <summary>
        /// Minimum package weight in kilograms
        /// </summary>
        [SwaggerSchema("Peso minimo en kg", Nullable = true)]
        public double? MinWeight { get; set; }

        /// <summary>
        /// Maximum package weight in kilograms
        /// </summary>
        [SwaggerSchema("Peso maximo en kg", Nullable = true)]
        public double? MaxWeight { get; set; }

        /// <summary>
        /// Minimum package price
        /// </summary>
        [SwaggerSchema("Precio minimo del paquete", Nullable = true)]
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// Maximum package price
        /// </summary>
        [SwaggerSchema("Precio maximo del paquete", Nullable = true)]
        public decimal? MaxPrice { get; set; }
    }
}