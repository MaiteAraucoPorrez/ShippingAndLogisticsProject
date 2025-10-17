namespace ShippingAndLogisticsManagement.Infrastructure.DTOS
{
    public class PackageDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public double Weight { get; set; }
        public int ShipmentId { get; set; }
        public decimal Price { get; set; }
    }
}
