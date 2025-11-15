namespace ShippingAndLogisticsManagement.Infrastructure.Queries
{
    /// <summary>
    /// Contiene todas las consultas SQL relacionadas con la entidad <see cref="Route"/>.
    /// Incluye consultas para SQL Server, MySQL y reportes especiales con Dapper.
    /// </summary>
    public static class RouteQueries
    {
        /// <summary>
        /// Consulta para SQL Server:
        /// Obtiene una lista de rutas ordenadas por ID descendente.
        /// </summary>
        public static string RouteQuerySqlServer = @"
            SELECT 
                r.Id,
                r.Origin,
                r.Destination,
                r.DistanceKm,
                r.BaseCost,
                r.IsActive
            FROM Routes r
            ORDER BY r.Id DESC
            OFFSET 0 ROWS FETCH NEXT @Limit ROWS ONLY;
        ";

        /// <summary>
        /// Consulta para MySQL:
        /// Obtiene las rutas ordenadas por ID descendente.
        /// </summary>
        public static string RouteQueryMySQL = @"
            SELECT 
                r.Id,
                r.Origin,
                r.Destination,
                r.DistanceKm,
                r.BaseCost,
                r.IsActive
            FROM Routes r
            ORDER BY r.Id DESC
            LIMIT @Limit;
        ";

        /// <summary>
        /// Obtiene solo las rutas activas disponibles para nuevos envíos.
        /// </summary>
        public static string GetActiveRoutes = @"
            SELECT * FROM Routes
            WHERE IsActive = 1
            ORDER BY Origin, Destination;
        ";

        /// <summary>
        /// Obtiene el ranking de rutas más utilizadas con estadísticas completas.
        /// Incluye: total de envíos, envíos activos/completados, ingresos, costo por km.
        /// </summary>
        public static string GetMostUsedRoutes = @"
            SELECT 
                r.Id AS RouteId,
                r.Origin,
                r.Destination,
                r.DistanceKm,
                r.BaseCost,
                r.IsActive,
                COUNT(s.Id) AS TotalShipments,
                SUM(CASE WHEN s.State != 'Delivered' THEN 1 ELSE 0 END) AS ActiveShipments,
                SUM(CASE WHEN s.State = 'Delivered' THEN 1 ELSE 0 END) AS CompletedShipments,
                ISNULL(SUM(s.TotalCost), 0) AS TotalRevenue,
                ISNULL(AVG(s.TotalCost), 0) AS AverageRevenue,
                CASE 
                    WHEN r.DistanceKm > 0 THEN r.BaseCost / r.DistanceKm 
                    ELSE 0 
                END AS CostPerKm
            FROM Routes r
            LEFT JOIN Shipments s ON r.Id = s.RouteId
            GROUP BY r.Id, r.Origin, r.Destination, r.DistanceKm, r.BaseCost, r.IsActive
            ORDER BY TotalShipments DESC, TotalRevenue DESC
            OFFSET 0 ROWS FETCH NEXT @Limit ROWS ONLY;
        ";

        /// <summary>
        /// Busca una ruta por origen y destino exactos.
        /// </summary>
        public static string GetByOriginDestination = @"
            SELECT * FROM Routes
            WHERE Origin = @Origin 
            AND Destination = @Destination;
        ";

        /// <summary>
        /// Cuenta cuántos envíos tiene asociados una ruta.
        /// </summary>
        public static string CountShipmentsByRoute = @"
            SELECT COUNT(*) 
            FROM Shipments
            WHERE RouteId = @RouteId;
        ";

        /// <summary>
        /// Obtiene las rutas con mejor rendimiento (mayor ingreso promedio por envío).
        /// </summary>
        public static string GetMostProfitableRoutes = @"
            SELECT 
                r.Id AS RouteId,
                r.Origin,
                r.Destination,
                r.DistanceKm,
                r.BaseCost,
                COUNT(s.Id) AS TotalShipments,
                ISNULL(AVG(s.TotalCost), 0) AS AverageRevenue,
                ISNULL(SUM(s.TotalCost), 0) AS TotalRevenue
            FROM Routes r
            LEFT JOIN Shipments s ON r.Id = s.RouteId
            WHERE s.State = 'Delivered'
            GROUP BY r.Id, r.Origin, r.Destination, r.DistanceKm, r.BaseCost
            HAVING COUNT(s.Id) > 0
            ORDER BY AverageRevenue DESC
            OFFSET 0 ROWS FETCH NEXT @Limit ROWS ONLY;
        ";

        /// <summary>
        /// Obtiene rutas sin envíos asociados (candidatas para eliminación).
        /// </summary>
        public static string GetUnusedRoutes = @"
            SELECT r.*
            FROM Routes r
            LEFT JOIN Shipments s ON r.Id = s.RouteId
            WHERE s.Id IS NULL
            ORDER BY r.Id;
        ";
    }
}
