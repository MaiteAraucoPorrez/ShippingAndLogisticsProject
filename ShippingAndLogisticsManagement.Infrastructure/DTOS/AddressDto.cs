using ShippingAndLogisticsManagement.Core.Enum;

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
        public AddressType Type { get; set; }
        public bool IsDefault { get; set; }
        public string? Reference { get; set; }
        public string? ContactName { get; set; }
        public string? ContactPhone { get; set; }
        public bool IsActive { get; set; } = true;
    }
}