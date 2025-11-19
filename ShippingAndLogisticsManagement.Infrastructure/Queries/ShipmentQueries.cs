namespace ShippingAndLogisticsManagement.Infrastructure.Queries
{
    /// <summary>
    /// Contiene todas las consultas SQL relacionadas con la entidad <see cref="Shipment"/>.
    /// Incluye consultas para SQL Server, MySQL y reportes especiales con Dapper.
    /// </summary>
    public static class ShipmentQueries
    {
        /// <summary>
        /// Consulta para SQL Server:
        /// Obtiene una lista de envíos ordenados por fecha de envío (ShippingDate) descendente.
        /// Aplica paginación con OFFSET-FETCH.
        /// </summary>
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

        /// <summary>
        /// Consulta para MySQL:
        /// Obtiene los envíos ordenados por fecha de envío descendente,
        /// limitando la cantidad de registros retornados con LIMIT.
        /// </summary>
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

        /// <summary>
        /// Devuelve los envíos más recientes junto con:
        /// - Nombre del cliente
        /// - Origen y destino de la ruta
        /// 
        /// Usa JOIN entre Shipments, Customers y Routes.
        /// Ideal para reportes o dashboards.
        /// </summary>
        public static string GetShipmentCustomerRoute = @"
            SELECT
                s.Id AS ShipmentId,
                s.TrackingNumber,
                s.ShippingDate,
                s.TotalCost,
                s.State,
                c.Name AS CustomerName,
                r.Id AS RouteId,
                r.Origin,
                r.Destination
            FROM Shipments s
            INNER JOIN Customers c ON s.CustomerId = c.Id
            INNER JOIN Routes r ON s.RouteId = r.Id
            ORDER BY s.ShippingDate DESC;
        ";

        /// <summary>
        /// Obtiene una lista de clientes que tienen más de tres envíos activos.
        /// Se consideran como "activos" aquellos en estado 'Pending' o 'InTransit'.
        /// </summary>
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

        /// <summary>
        /// Devuelve las rutas marcadas como inactivas.
        /// Útil para auditorías, mantenimiento del sistema o reportes administrativos.
        /// </summary>
        public static string GetInactiveRoutes = @"
            SELECT 
                Id AS RouteId, 
                Origin, 
                Destination,
                DistanceKm,
                BaseCost
            FROM Routes 
            WHERE IsActive = 0;
        ";
    }
}