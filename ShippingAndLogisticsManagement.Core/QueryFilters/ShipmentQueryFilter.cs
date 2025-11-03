using Swashbuckle.AspNetCore.Annotations;

namespace ShippingAndLogisticsManagement.Core.QueryFilters
{
    /// <summary>
    /// Represents a set of criteria for querying shipments, including pagination and specific shipment attributes.
    /// </summary>
    /// <remarks>This filter allows querying shipments based on various attributes such as shipping date,
    /// state, customer ID, route ID, total cost, and tracking number. It extends the <see
    /// cref="PaginationQueryFilter"/> to include pagination capabilities.</remarks>
    public class ShipmentQueryFilter: PaginationQueryFilter
    {
        /// <summary>
        /// Gets or sets the date when the item is scheduled to be shipped.
        /// </summary>
        [SwaggerSchema("Fecha del envio", Format = "date-time", Nullable = true)]
        public DateTime? ShippingDate { get; set; }
        /// <summary>
        /// Gets or sets the state of the entity.
        /// </summary>
        [SwaggerSchema("Estado del envio", Nullable = true)]
        public required string State { get; set; }
        /// <summary>
        /// Gets or sets the unique identifier for the customer.
        /// </summary>
        [SwaggerSchema("Identificador del cliente", Nullable = true)]
        public int ? CustomerId { get; set; }
        /// <summary>
        /// Gets or sets the identifier for the route.
        /// </summary>
        [SwaggerSchema("Identificador de la ruta", Nullable = true)]
        public int ? RouteId { get; set; }
        /// <summary>
        /// Gets or sets the total cost of the items.
        /// </summary>
        [SwaggerSchema("Costo total del envio", Nullable = true)]
        public double TotalCost { get; set; }
        /// <summary>
        /// Gets or sets the tracking number associated with the shipment.
        /// </summary>
        [SwaggerSchema("Numero de seguimiento para rastrear el envio", Nullable = true)]
        public string TrackingNumber { get; set; } = null!;
    }
}
