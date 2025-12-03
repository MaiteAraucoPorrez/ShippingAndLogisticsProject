namespace ShippingAndLogisticsManagement.Infrastructure.Queries
{
    public static class VehicleQueries
    {
        public static string GetRecentVehiclesSqlServer = @"
            SELECT TOP (@Limit) *
            FROM Vehicles
            ORDER BY Id DESC;
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
            ORDER BY Type;
        ";

        public static string GetAvailableVehicles = @"
            SELECT *
            FROM Vehicles
            WHERE IsActive = 1
            AND Status = @AvailableStatus
            AND AssignedDriverId IS NULL
            ORDER BY Type, MaxWeightCapacityKg DESC;
        ";

        public static string GetByType = @"
            SELECT *
            FROM Vehicles
            WHERE Type = @Type
            AND IsActive = 1
            ORDER BY Type;
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
            AND Status = @AvailableStatus
            AND (MaxWeightCapacityKg - CurrentWeightKg) >= @RequiredWeight
            AND (MaxVolumeCapacityM3 - CurrentVolumeM3) >= @RequiredVolume
            ORDER BY 
                (MaxWeightCapacityKg - CurrentWeightKg) DESC,
                (MaxVolumeCapacityM3 - CurrentVolumeM3) DESC;
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
                (v.MaxVolumeCapacityM3 - v.CurrentVolumeM3) AS AvailableVolumeM3

            FROM Vehicles v
            WHERE v.Id = @VehicleId;
        ";
    }
}