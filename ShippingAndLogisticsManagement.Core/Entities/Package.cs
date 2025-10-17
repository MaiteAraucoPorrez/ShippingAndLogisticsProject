namespace ShippingAndLogisticsManagement.Core.Entities
{
    public class Package
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public double Weight { get; set; }
        public int ShipmentId { get; set; }
        public decimal Price { get; set; }

        public Shipment Shipment { get; set; } = null!;
    }
}
