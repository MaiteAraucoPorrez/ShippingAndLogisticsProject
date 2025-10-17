namespace ShippingAndLogisticsManagement.Core.Entities
{
    public class Shipment
    {
        public int Id { get; set; }
        public DateTime ShippingDate { get; set; }
        public required string State { get; set; }
        public int CustomerId { get; set; }
        public int RouteId { get; set; }
        public double TotalCost { get; set; }
        public string TrackingNumber { get; set; } = null!;

        public Customer Customer { get; set; } = null!;
        public ICollection<Package> Packages { get; set; } = new List<Package>();
    }
}
