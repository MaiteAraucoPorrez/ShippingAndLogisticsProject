namespace ShippingAndLogisticsManagement.Core.CustomEntities
{
    /// <summary>
    /// Historial de paso de un envío por almacenes
    /// </summary>
    public class ShipmentWarehouseHistoryResponse
    {
        public int ShipmentId { get; set; }
        public string TrackingNumber { get; set; } = null!;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;
        public string City { get; set; } = null!;
        public DateTime EntryDate { get; set; }
        public DateTime? ExitDate { get; set; }
        public string Status { get; set; } = null!;
        public double? StayTimeHours { get; set; }
        public string? StorageLocation { get; set; }
    }
}