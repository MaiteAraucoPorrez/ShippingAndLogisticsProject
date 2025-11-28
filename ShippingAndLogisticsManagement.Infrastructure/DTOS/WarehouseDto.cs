namespace ShippingAndLogisticsManagement.Infrastructure.DTOS
{
    public class WarehouseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public double MaxCapacityM3 { get; set; }
        public double CurrentCapacityM3 { get; set; }
        public bool IsActive { get; set; } = true;
        public string Type { get; set; } = null!; // "Central", "Regional", "Local"
        public string? OperatingHours { get; set; }
        public string? ManagerName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime OpeningDate { get; set; }
        public string? Notes { get; set; }
    }
}