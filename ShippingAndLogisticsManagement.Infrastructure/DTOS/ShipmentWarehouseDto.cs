namespace ShippingAndLogisticsManagement.Infrastructure.DTOS
{
    public class ShipmentWarehouseDto
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public int WarehouseId { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime? ExitDate { get; set; }
        public string Status { get; set; } = null!; // "Received", "InStorage", "Processing", "Dispatched"
        public string? ReceivedBy { get; set; }
        public string? DispatchedBy { get; set; }
        public string? Notes { get; set; }
        public string? StorageLocation { get; set; }
    }
}