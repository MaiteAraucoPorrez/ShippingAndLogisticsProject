namespace ShippingAndLogisticsManagement.Infrastructure.DTOS
{
    public class VehicleDto
    {
        public int Id { get; set; }
        public string PlateNumber { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public string? Color { get; set; }
        public string Type { get; set; } = null!;
        public double MaxWeightCapacityKg { get; set; }
        public double MaxVolumeCapacityM3 { get; set; }
        public double CurrentWeightKg { get; set; }
        public double CurrentVolumeM3 { get; set; }
        public string Status { get; set; } = "Available";
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public int CurrentMileage { get; set; }
        public int? LastMaintenanceMileage { get; set; }
        public string? FuelType { get; set; }
        public double? FuelConsumptionPer100Km { get; set; }
        public string? VIN { get; set; }
        public string? InsurancePolicyNumber { get; set; }
        public DateTime? InsuranceExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int? BaseWarehouseId { get; set; }
        public int? AssignedDriverId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string? Notes { get; set; }
    }
}