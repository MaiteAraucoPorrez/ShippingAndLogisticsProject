namespace ShippingAndLogisticsManagement.Core.CustomEntities
{
    /// <summary>
    /// Represents a detailed package record including shipment, customer, and route data.
    /// </summary>
    public class PackageDetailResponse
    {
        public int PackageId { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Weight { get; set; }
        public double Price { get; set; }

        public int ShipmentId { get; set; }
        public string ShipmentState { get; set; } = string.Empty;
        public DateTime ShippingDate { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        public int RouteId { get; set; }
        public string RouteName { get; set; } = string.Empty;
    }
}
