namespace ShippingAndLogisticsManagement.Infrastructure.Queries
{
    public static class WarehouseQueries
    {
        public static string GetRecentWarehousesSqlServer = @"
            SELECT TOP (@Limit) *
            FROM Warehouses
            ORDER BY Id DESC;
        ";

        public static string GetById = @"
            SELECT *
            FROM Warehouses
            WHERE Id = @Id;
        ";

        public static string GetByCode = @"
            SELECT *
            FROM Warehouses
            WHERE Code = @Code;
        ";

        public static string GetActiveWarehouses = @"
            SELECT *
            FROM Warehouses
            WHERE IsActive = 1
            ORDER BY Type, City;
        ";

        public static string GetByCity = @"
            SELECT *
            FROM Warehouses
            WHERE City LIKE '%' + @City + '%'
            AND IsActive = 1
            ORDER BY Name;
        ";

        public static string GetByDepartment = @"
            SELECT *
            FROM Warehouses
            WHERE Department = @Department
            AND IsActive = 1
            ORDER BY City, Name;
        ";

        public static string GetByType = @"
            SELECT *
            FROM Warehouses
            WHERE Type = @Type
            AND IsActive = 1
            ORDER BY Department, City;
        ";

        public static string GetAvailableWarehouses = @"
            SELECT *
            FROM Warehouses
            WHERE IsActive = 1
            AND (MaxCapacityM3 - CurrentCapacityM3) >= @RequiredCapacity
            ORDER BY (MaxCapacityM3 - CurrentCapacityM3) DESC;
        ";

        public static string CodeExists = @"
            SELECT COUNT(*)
            FROM Warehouses
            WHERE Code = @Code
            AND (@ExcludeWarehouseId IS NULL OR Id != @ExcludeWarehouseId);
        ";

        public static string UpdateCurrentCapacity = @"
            UPDATE Warehouses
            SET CurrentCapacityM3 = @NewCapacity
            WHERE Id = @WarehouseId;
        ";

        public static string GetWarehouseStatistics = @"
            SELECT 
                w.Id AS WarehouseId,
                w.Name AS WarehouseName,
                w.Code,
                w.City,
                COUNT(sw.Id) AS TotalShipments,
                
                -- Envíos actuales (los que no tienen fecha de salida)
                SUM(CASE WHEN sw.ExitDate IS NULL THEN 1 ELSE 0 END) AS CurrentShipments,
                
                -- Envíos despachados
                SUM(CASE WHEN sw.ExitDate IS NOT NULL THEN 1 ELSE 0 END) AS DispatchedShipments,
                
                -- Porcentaje de Ocupación (Con protección contra división por cero)
                CAST((w.CurrentCapacityM3 / NULLIF(w.MaxCapacityM3, 0)) * 100 AS DECIMAL(5,2)) AS OccupancyPercentage,
                
                -- Capacidad Disponible
                (w.MaxCapacityM3 - w.CurrentCapacityM3) AS AvailableCapacity,
                
                -- Tiempo promedio de permanencia (en horas)
                AVG(CASE 
                    WHEN sw.ExitDate IS NOT NULL 
                    THEN DATEDIFF(HOUR, sw.EntryDate, sw.ExitDate) 
                    ELSE NULL 
                END) AS AverageStayTimeHours

            FROM Warehouses w
            LEFT JOIN ShipmentWarehouses sw ON w.Id = sw.WarehouseId
            WHERE w.Id = @WarehouseId
            GROUP BY w.Id, w.Name, w.Code, w.City, w.MaxCapacityM3, w.CurrentCapacityM3;
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

        public static string GetCurrentShipmentsInWarehouse = @"
            SELECT sw.*
            FROM ShipmentWarehouses sw
            WHERE sw.WarehouseId = @WarehouseId
            AND sw.ExitDate IS NULL
            ORDER BY sw.EntryDate DESC;
        ";

        public static string GetCurrentWarehouseForShipment = @"
            SELECT TOP 1 *
            FROM ShipmentWarehouses
            WHERE ShipmentId = @ShipmentId
            AND ExitDate IS NULL
            ORDER BY EntryDate DESC;
        ";

        public static string IsShipmentInWarehouse = @"
            SELECT COUNT(*)
            FROM ShipmentWarehouses
            WHERE ShipmentId = @ShipmentId
            AND ExitDate IS NULL;
        ";
    }
}