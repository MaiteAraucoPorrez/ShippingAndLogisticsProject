namespace ShippingAndLogisticsManagement.Infrastructure.Queries
{
    public static class DriverQueries
    {
        public static string GetRecentDriversSqlServer = @"
            SELECT TOP (@Limit) *
            FROM Drivers
            ORDER BY CreatedAt DESC, Id DESC;
        ";

        public static string GetRecentDriversMySQL = @"
            SELECT *
            FROM Drivers
            ORDER BY CreatedAt DESC, Id DESC
            LIMIT @Limit;
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

        public static string GetByIdentityDocument = @"
            SELECT *
            FROM Drivers
            WHERE IdentityDocument = @IdentityDocument;
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
            AND Status = 'Available'
            AND CurrentVehicleId IS NULL
            AND LicenseExpiryDate > GETDATE()
            ORDER BY YearsOfExperience DESC, AverageRating DESC;
        ";

        public static string GetByStatus = @"
            SELECT *
            FROM Drivers
            WHERE Status = @Status
            AND IsActive = 1
            ORDER BY FullName;
        ";

        public static string GetDriversWithExpiringLicenses = @"
            SELECT *
            FROM Drivers
            WHERE IsActive = 1
            AND DATEDIFF(DAY, GETDATE(), LicenseExpiryDate) BETWEEN 0 AND @DaysThreshold
            ORDER BY LicenseExpiryDate ASC;
        ";

        public static string LicenseNumberExists = @"
            SELECT COUNT(*)
            FROM Drivers
            WHERE LicenseNumber = @LicenseNumber
            AND (@ExcludeDriverId IS NULL OR Id != @ExcludeDriverId);
        ";

        public static string IdentityDocumentExists = @"
            SELECT COUNT(*)
            FROM Drivers
            WHERE IdentityDocument = @IdentityDocument
            AND (@ExcludeDriverId IS NULL OR Id != @ExcludeDriverId);
        ";

        public static string GetDriverStatistics = @"
            SELECT 
                d.Id AS DriverId,
                d.FullName,
                d.LicenseNumber,
                d.TotalDeliveries,
                0 AS OnTimeDeliveries,
                0 AS LateDeliveries,
                0.0 AS OnTimePercentage,
                d.AverageRating,
                d.YearsOfExperience,
                DATEDIFF(DAY, GETDATE(), d.LicenseExpiryDate) AS DaysUntilLicenseExpiry,
                CASE WHEN DATEDIFF(DAY, GETDATE(), d.LicenseExpiryDate) <= 30 
                     THEN 1 ELSE 0 END AS LicenseExpiringSoon
            FROM Drivers d
            WHERE d.Id = @DriverId;
        ";
    }
}
