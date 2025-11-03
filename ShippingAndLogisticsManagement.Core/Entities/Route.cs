namespace ShippingAndLogisticsManagement.Core.Entities
{
    public partial class Route: BaseEntity
    {
       // public int Id { get; set; }
        public string Origin { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public double DistanceKm { get; set; }
        public double BaseCost { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
