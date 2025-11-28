namespace ShippingAndLogisticsManagement.Infrastructure.Queries
{
    public static class VehicleQueries
    {
        public static string GetRecentVehiclesSqlServer = @"
            SELECT TOP (@Limit) *
            FROM Vehicles
            ORDER BY CreatedAt DESC, Id DESC;
        ";

        public static string GetRecentVehiclesMySQL = @"
            SELECT *
            FROM Vehicles
            ORDER BY CreatedAt DESC, Id DESC
            LIMIT @Limit;
        ";

        public static string GetById = @"
            SELECT *
            FROM Vehicles
            WHERE Id = @Id;
        ";

        public static string GetByPlateNumber = @"
            SELECT *
            FROM Vehicles
            WHERE PlateNumber = @PlateNumber;
        ";

        public static string GetActiveVehicles = @"
            SELECT *
            FROM Vehicles
            WHERE IsActive = 1
            ORDER BY Type, Brand, Model;
        ";

        public static string GetAvailableVehicles = @"
            SELECT *
            FROM Vehicles
            WHERE IsActive = 1
            AND Status = 'Available'
            AND AssignedDriverId IS NULL
            ORDER BY Type, MaxWeightCapacityKg DESC;
        ";

        public static string GetByType = @"
            SELECT *
            FROM Vehicles
            WHERE Type = @Type
            AND IsActive = 1
            ORDER BY Brand, Model;
        ";

        public static string GetByStatus = @"
            SELECT *
            FROM Vehicles
            WHERE Status = @Status
            AND IsActive = 1
            ORDER BY PlateNumber;
        ";

        public static string GetByWarehouse = @"
            SELECT *
            FROM Vehicles
            WHERE BaseWarehouseId = @WarehouseId
            AND IsActive = 1
            ORDER BY Type, PlateNumber;
        ";

        public static string GetByCapacity = @"
            SELECT *
            FROM Vehicles
            WHERE IsActive = 1
            AND Status = 'Available'
            AND (MaxWeightCapacityKg - CurrentWeightKg) >= @RequiredWeight
            AND (MaxVolumeCapacityM3 - CurrentVolumeM3) >= @RequiredVolume
            ORDER BY 
                (MaxWeightCapacityKg - CurrentWeightKg) DESC,
                (MaxVolumeCapacityM3 - CurrentVolumeM3) DESC;
        ";

        public static string GetVehiclesRequiringMaintenance = @"
            SELECT *
            FROM Vehicles
            WHERE IsActive = 1
            AND (
                NextMaintenanceDate IS NOT NULL 
                AND NextMaintenanceDate <= DATEADD(DAY, 7, GETDATE())
                OR 
                (LastMaintenanceMileage IS NOT NULL 
                 AND CurrentMileage - LastMaintenanceMileage >= 10000)
            )
            ORDER BY NextMaintenanceDate ASC;
        ";

        public static string PlateNumberExists = @"
            SELECT COUNT(*)
            FROM Vehicles
            WHERE PlateNumber = @PlateNumber
            AND (@ExcludeVehicleId IS NULL OR Id != @ExcludeVehicleId);
        ";

        public static string UpdateCurrentLoad = @"
            UPDATE Vehicles
            SET CurrentWeightKg = @Weight,
                CurrentVolumeM3 = @Volume
            WHERE Id = @VehicleId;
        ";

        public static string GetVehicleStatistics = @"
            SELECT 
                v.Id AS VehicleId,
                v.PlateNumber,
                v.Brand,
                v.Model,
                0 AS TotalShipments,
                0 AS CurrentShipments,
                0 AS CompletedShipments,
                CASE WHEN v.MaxWeightCapacityKg > 0 
                     THEN CAST((v.CurrentWeightKg / v.MaxWeightCapacityKg) * 100 AS DECIMAL(5,2))
                     ELSE 0 END AS WeightOccupancyPercentage,
                CASE WHEN v.MaxVolumeCapacityM3 > 0 
                     THEN CAST((v.CurrentVolumeM3 / v.MaxVolumeCapacityM3) * 100 AS DECIMAL(5,2))
                     ELSE 0 END AS VolumeOccupancyPercentage,
                (v.MaxWeightCapacityKg - v.CurrentWeightKg) AS AvailableWeightKg,
                (v.MaxVolumeCapacityM3 - v.CurrentVolumeM3) AS AvailableVolumeM3,
                CASE WHEN v.LastMaintenanceMileage IS NOT NULL 
                     THEN v.CurrentMileage - v.LastMaintenanceMileage 
                     ELSE NULL END AS KmSinceLastMaintenance,
                CASE WHEN v.LastMaintenanceDate IS NOT NULL 
                     THEN DATEDIFF(DAY, v.LastMaintenanceDate, GETDATE())
                     ELSE NULL END AS DaysSinceLastMaintenance,
                CASE 
                    WHEN v.NextMaintenanceDate IS NOT NULL 
                         AND v.NextMaintenanceDate <= DATEADD(DAY, 7, GETDATE())
                         THEN 1
                    WHEN v.LastMaintenanceMileage IS NOT NULL 
                         AND v.CurrentMileage - v.LastMaintenanceMileage >= 10000
                         THEN 1
                    ELSE 0 
                END AS RequiresMaintenance
            FROM Vehicles v
            WHERE v.Id = @VehicleId;
        ";
    }
}