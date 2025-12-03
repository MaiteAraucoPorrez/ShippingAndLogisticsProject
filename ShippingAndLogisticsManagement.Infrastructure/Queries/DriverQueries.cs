namespace ShippingAndLogisticsManagement.Infrastructure.Queries
{
    public static class DriverQueries
    {
        public static string GetRecentDriversSqlServer = @"
            SELECT TOP (@Limit) *
            FROM Drivers
            ORDER BY Id DESC;
        ";

        public static string GetById = @"
            SELECT *
            FROM Drivers
            WHERE Id = @Id;
        ";

        public static string GetByLicenseNumber = @"
            SELECT *
            FROM Drivers
            WHERE LicenseNumber = @LicenseNumber;
        ";

        public static string GetActiveDrivers = @"
            SELECT *
            FROM Drivers
            WHERE IsActive = 1
            ORDER BY FullName;
        ";

        public static string GetAvailableDrivers = @"
            SELECT *
            FROM Drivers
            WHERE IsActive = 1
            AND Status = @AvailableStatus
            AND CurrentVehicleId IS NULL
            ORDER BY FullName;
        ";

        public static string GetByStatus = @"
            SELECT *
            FROM Drivers
            WHERE Status = @Status
            AND IsActive = 1
            ORDER BY FullName;
        ";

        public static string LicenseNumberExists = @"
            SELECT COUNT(*)
            FROM Drivers
            WHERE LicenseNumber = @LicenseNumber
            AND (@ExcludeDriverId IS NULL OR Id != @ExcludeDriverId);
        ";

        public static string GetDriverStatistics = @"
            SELECT 
                d.Id AS DriverId,
                d.FullName,
                d.LicenseNumber,
                d.TotalDeliveries,
                d.Status,
                CASE WHEN d.CurrentVehicleId IS NOT NULL THEN 1 ELSE 0 END AS HasVehicleAssigned
            FROM Drivers d
            WHERE d.Id = @DriverId;
        ";
    }
}
