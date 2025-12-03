using ShippingAndLogisticsManagement.Core.Enum;

namespace ShippingAndLogisticsManagement.Infrastructure.DTOS
{
    public class VehicleDto
    {
        public int Id { get; set; }
        public string PlateNumber { get; set; } = null!;
        public VehicleType Type { get; set; }
        public double MaxWeightCapacityKg { get; set; }
        public double MaxVolumeCapacityM3 { get; set; }
        public double CurrentWeightKg { get; set; }
        public double CurrentVolumeM3 { get; set; }
        public VehicleStatus Status { get; set; }
        public string? VIN { get; set; }
        public bool IsActive { get; set; } = true;
        public int? BaseWarehouseId { get; set; }
        public int? AssignedDriverId { get; set; }
    }
}