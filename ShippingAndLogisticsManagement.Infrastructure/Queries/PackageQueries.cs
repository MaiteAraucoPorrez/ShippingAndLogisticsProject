namespace ShippingAndLogisticsManagement.Infrastructure.Queries
{
    public static class PackageQueries
    {
        public static string GetAllDapper = @"
            SELECT * FROM Package
            ORDER BY Id DESC
            LIMIT @Limit;
        ";

        public static string GetByIdDapper = @"
            SELECT * FROM Package
            WHERE Id = @Id;
        ";

        public static string GetByShipmentId = @"
            SELECT * FROM Package
            WHERE ShipmentId = @ShipmentId;
        ";

        public static string GetPackageSummary = @"
            SELECT 
                COUNT(Id) AS TotalPackages,
                SUM(Weight) AS TotalWeight,
                SUM(Price) AS TotalValue,
                AVG(Weight) AS AvgWeight,
                AVG(Price) AS AvgValue
            FROM Package
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
            FROM Package p
            INNER JOIN Shipment s ON p.ShipmentId = s.Id
            INNER JOIN Customer c ON s.CustomerId = c.Id
            INNER JOIN Route r ON s.RouteId = r.Id;
        ";

        public static string GetHeavyPackages = @"
            SELECT * FROM Package
            WHERE Weight > @MinWeight
            ORDER BY Weight DESC;
        ";
    }
}
