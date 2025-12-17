namespace ShippingAndLogisticsManagement.Infrastructure.Queries
{
    /// <summary>
    /// Consultas SQL para ShipmentWarehouse
    /// </summary>
    public static class ShipmentWarehouseQueries
    {
        public static string GetAll = @"
            SELECT * FROM ShipmentWarehouses
            ORDER BY EntryDate DESC;
        ";

        public static string GetById = @"
            SELECT * FROM ShipmentWarehouses
            WHERE Id = @Id;
        ";

        public static string GetByShipmentId = @"
            SELECT * FROM ShipmentWarehouses
            WHERE ShipmentId = @ShipmentId
            ORDER BY EntryDate DESC;
        ";

        public static string GetByWarehouseId = @"
            SELECT * FROM ShipmentWarehouses
            WHERE WarehouseId = @WarehouseId
            ORDER BY EntryDate DESC;
        ";

        public static string GetCurrentShipmentsInWarehouse = @"
            SELECT * FROM ShipmentWarehouses
            WHERE WarehouseId = @WarehouseId
            AND ExitDate IS NULL
            ORDER BY EntryDate DESC;
        ";

        public static string GetCurrentLocationForShipment = @"
            SELECT TOP 1 * FROM ShipmentWarehouses
            WHERE ShipmentId = @ShipmentId
            AND ExitDate IS NULL
            ORDER BY EntryDate DESC;
        ";

        public static string IsShipmentInWarehouse = @"
            SELECT COUNT(*) FROM ShipmentWarehouses
            WHERE ShipmentId = @ShipmentId
            AND ExitDate IS NULL;
        ";

        public static string GetShipmentHistory = @"
            SELECT 
                sw.ShipmentId,
                s.TrackingNumber,
                sw.WarehouseId,
                w.Name AS WarehouseName,
                w.City,
                sw.EntryDate,
                sw.ExitDate,
                sw.Status,
                CASE 
                    WHEN sw.ExitDate IS NOT NULL 
                    THEN DATEDIFF(HOUR, sw.EntryDate, sw.ExitDate)
                    ELSE DATEDIFF(HOUR, sw.EntryDate, GETDATE())
                END AS StayTimeHours,
                sw.StorageLocation
            FROM ShipmentWarehouses sw
            INNER JOIN Warehouses w ON sw.WarehouseId = w.Id
            INNER JOIN Shipments s ON sw.ShipmentId = s.Id
            WHERE sw.ShipmentId = @ShipmentId
            ORDER BY sw.EntryDate ASC;
        ";

        public static string UpdateStatus = @"
            UPDATE ShipmentWarehouses
            SET Status = @Status
            WHERE Id = @Id;
        ";

        public static string RegisterExit = @"
            UPDATE ShipmentWarehouses
            SET ExitDate = @ExitDate,
                DispatchedBy = @DispatchedBy,
                Status = 'Dispatched'
            WHERE Id = @Id;
        ";
    }
}