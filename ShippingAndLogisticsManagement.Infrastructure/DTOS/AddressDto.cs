namespace ShippingAndLogisticsManagement.Infrastructure.DTOS
{
    public class AddressDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string? Zone { get; set; }
        public string? PostalCode { get; set; }
        public string Type { get; set; } = null!; // "Pickup" o "Delivery"
        public bool IsDefault { get; set; }
        public string? Reference { get; set; }
        public string? Alias { get; set; }
        public string? ContactName { get; set; }
        public string? ContactPhone { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsActive { get; set; } = true;
    }
}