namespace ShippingAndLogisticsManagement.Core.CustomEntities
{
    public class ShipmentCustomerRouteResponse
    {
        public int ShipmentId { get; set; }
        public string TrackingNumber { get; set; }
        public string CustomerName { get; set; }
        public int RouteId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime ShippingDate { get; set; }
        public string State { get; set; }
        public decimal TotalCost { get; set; }
    }
}