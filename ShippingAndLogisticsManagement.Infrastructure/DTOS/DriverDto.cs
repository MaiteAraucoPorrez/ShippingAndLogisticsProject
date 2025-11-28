namespace ShippingAndLogisticsManagement.Infrastructure.DTOS
{
    public class DriverDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string IdentityDocument { get; set; } = null!;
        public string LicenseNumber { get; set; } = null!;
        public string LicenseCategory { get; set; } = null!;
        public DateTime LicenseIssueDate { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public string Phone { get; set; } = null!;
        public string? AlternativePhone { get; set; }
        public string Email { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public int YearsOfExperience { get; set; }
        public bool IsActive { get; set; } = true;
        public string Status { get; set; } = "Available";
        public int? CurrentVehicleId { get; set; }
        public double? AverageRating { get; set; }
        public int TotalDeliveries { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? BloodType { get; set; }
        public string? Notes { get; set; }
    }
}