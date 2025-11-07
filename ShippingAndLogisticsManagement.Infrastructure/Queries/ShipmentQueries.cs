namespace ShippingAndLogisticsManagement.Infrastructure.Queries
{
    public static class ShipmentQueries
    {
        public static string ShipmentQuerySqlServer = @"
            SELECT 
                s.Id,
                s.TrackingNumber,
                s.ShippingDate,
                s.State,
                s.TotalCost,
                s.CustomerId,
                s.RouteId
            FROM Shipments s
            ORDER BY s.ShippingDate DESC
            OFFSET 0 ROWS FETCH NEXT @Limit ROWS ONLY;
        ";

        public static string ShipmentQueryMySQL = @"
            SELECT 
                s.Id,
                s.TrackingNumber,
                s.ShippingDate,
                s.State,
                s.TotalCost,
                s.CustomerId,
                s.RouteId
            FROM Shipments s
            ORDER BY s.ShippingDate DESC
            LIMIT @Limit;
        ";

        // Devuelve los envíos más recientes junto con el nombre del cliente y la ruta
        public static string RecentShipmentsWithCustomerAndRoute = @"
            SELECT
                s.Id AS ShipmentId,
                s.TrackingNumber,
                s.ShippingDate,
                s.TotalCost,
                s.State,
                c.Name AS CustomerName,
                r.Origin,
                r.Destination
            FROM Shipments s
            INNER JOIN Customers c ON s.CustomerId = c.Id
            INNER JOIN Routes r ON s.RouteId = r.Id
            ORDER BY s.ShippingDate DESC
            OFFSET @Offset ROWS
            FETCH NEXT @Limit ROWS ONLY;
        ";

        // Clientes con más de 3 envíos activos (pendientes o en tránsito)
        public static string CustomersWithMoreThan3ActiveShipments = @"
            SELECT 
                c.Id AS CustomerId,
                c.Name AS CustomerName,
                COUNT(s.Id) AS ActiveShipments
            FROM Shipments s
            INNER JOIN Customers c ON s.CustomerId = c.Id
            WHERE s.State IN ('Pending', 'InTransit')
            GROUP BY c.Id, c.Name
            HAVING COUNT(s.Id) > 3;
        ";

        // --- Consulta para verificar rutas inactivas ---
        public static string InactiveRoutes = @"
            SELECT 
                Id AS RouteId, 
                Origin, 
                Destination 
            FROM Routes 
            WHERE IsActive = 0;
        ";
    }
}

