namespace ShippingAndLogisticsManagement.Infrastructure.Queries
{
    public static class PackageQueries
    {
        public static string PackageQuerySqlServer = @"
            SELECT 
                p.Id,
                p.Description,
                p.Weight,
                p.Price,
                p.ShipmentId
            FROM Packages p
            ORDER BY p.Id DESC
            OFFSET 0 ROWS FETCH NEXT @Limit ROWS ONLY;
        ";
        public static string PackageQueryMySQL = @"
            SELECT 
                p.Id,
                p.Description,
                p.Weight,
                p.Price,
                p.ShipmentId
            FROM Packages p
            ORDER BY p.Id DESC
            LIMIT @Limit;
        ";

        public static string GetAllDapper = @"
        SELECT * FROM Packages
            ORDER BY Id DESC
            OFFSET 0 ROWS FETCH NEXT @Limit ROWS ONLY;
        ";

        public static string GetByIdDapper = @"
            SELECT * FROM Packages
            WHERE Id = @Id;
        ";

        public static string GetByShipmentId = @"
            SELECT * FROM Packages
            WHERE ShipmentId = @ShipmentId;
        ";

        public static string GetPackageSummary = @"
            SELECT 
                COUNT(Id) AS TotalPackages,
                SUM(Weight) AS TotalWeight,
                SUM(Price) AS TotalValue,
                AVG(Weight) AS AvgWeight,
                AVG(Price) AS AvgValue
            FROM Packages
            WHERE ShipmentId = @ShipmentId;
        ";

        public static string GetPackageWithDetails = @"
            SELECT 
                p.Id AS PackageId,
                p.Description,
                p.Weight,
                p.Price,
                s.Id AS ShipmentId,
                s.State AS ShipmentState,
                s.ShippingDate,
                c.Id AS CustomerId,
                c.Name AS CustomerName,
                r.Id AS RouteId,
                r.RouteName
            FROM Packages p
            INNER JOIN Shipments s ON p.ShipmentId = s.Id
            INNER JOIN Customers c ON s.CustomerId = c.Id
            INNER JOIN Routes r ON s.RouteId = r.Id;
        ";

        public static string GetHeavyPackages = @"
            SELECT * FROM Packages
            WHERE Weight > @MinWeight
            ORDER BY Weight DESC;
        ";
    }
}
