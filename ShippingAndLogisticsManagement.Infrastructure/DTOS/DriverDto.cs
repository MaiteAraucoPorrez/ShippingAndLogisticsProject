using ShippingAndLogisticsManagement.Core.Enum;

namespace ShippingAndLogisticsManagement.Infrastructure.DTOS
{
    public class DriverDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string LicenseNumber { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; } = true;
        public DriverStatus Status { get; set; }
        public int? CurrentVehicleId { get; set; }
        public int TotalDeliveries { get; set; }
    }
}