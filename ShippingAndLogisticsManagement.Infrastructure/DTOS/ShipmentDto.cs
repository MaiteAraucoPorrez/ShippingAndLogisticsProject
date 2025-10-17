namespace ShippingAndLogisticsManagement.Infrastructure.DTOS
{
    public class ShipmentDto
    {
        public int Id { get; set; }
        public string ShippingDate { get; set; }
        public string State { get; set; }
        public int CustomerId { get; set; }
        public int RouteId { get; set; }
        public double TotalCost { get; set; }
        public string TrackingNumber { get; set; } = null!;

        public List<PackageDto> Packages { get; set; } = new();

        public class PackageDto
        {
            public int Id { get; set; }
            public string Description { get; set; } = null!;
            public double Weight { get; set; }
            public decimal Price { get; set; }
        }
    }
}

