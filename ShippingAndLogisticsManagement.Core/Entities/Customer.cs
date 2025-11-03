namespace ShippingAndLogisticsManagement.Core.Entities
{
    public partial class Customer: BaseEntity
    {
        //public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
}
