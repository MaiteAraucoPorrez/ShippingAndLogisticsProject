namespace ShippingAndLogisticsManagement.Infrastructure.Queries
{
    /// <summary>
    /// Contiene todas las consultas SQL relacionadas con la entidad <see cref="Package"/>.
    /// Incluye versiones específicas para SQL Server y MySQL, así como consultas
    /// especializadas para reportes, resúmenes y operaciones con Dapper.
    /// </summary>
    public static class PackageQueries
    {
        /// <summary>
        /// Consulta para SQL Server:
        /// Obtiene una lista de paquetes ordenados por ID descendente,
        /// utilizando paginación con OFFSET-FETCH.
        /// </summary>
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

        /// <summary>
        /// Consulta para MySQL:
        /// Devuelve los paquetes ordenados por ID descendente,
        /// limitando la cantidad de resultados con LIMIT.
        /// </summary>
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

        /// <summary>
        /// Consulta general para obtener todos los paquetes (Dapper).
        /// Utiliza paginación para limitar la cantidad de registros.
        /// </summary>
        public static string GetAllDapper = @"
            SELECT * FROM Packages
            ORDER BY Id DESC
            OFFSET 0 ROWS FETCH NEXT @Limit ROWS ONLY;
        ";

        /// <summary>
        /// Obtiene un paquete específico por su identificador.
        /// </summary>
        public static string GetByIdDapper = @"
            SELECT * FROM Packages
            WHERE Id = @Id;
        ";

        /// <summary>
        /// Obtiene todos los paquetes asociados a un envío específico (ShipmentId).
        /// </summary>
        public static string GetByShipmentId = @"
            SELECT * FROM Packages
            WHERE ShipmentId = @ShipmentId;
        ";

        /// <summary>
        /// Devuelve un resumen estadístico de los paquetes pertenecientes a un envío.
        /// Incluye: cantidad total, peso total, valor total y promedios.
        /// </summary>
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

        /// <summary>
        /// Obtiene información detallada de cada paquete, incluyendo:
        /// - Datos del paquete
        /// - Información del envío asociado
        /// - Cliente que realizó el envío
        /// - Ruta correspondiente
        /// 
        /// Usa múltiples INNER JOIN para obtener una vista completa de los datos.
        /// </summary>
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
                CONCAT(r.Origin, ' → ', r.Destination) AS RouteName
            FROM Packages p
            INNER JOIN Shipments s ON p.ShipmentId = s.Id
            INNER JOIN Customers c ON s.CustomerId = c.Id
            INNER JOIN Routes r ON s.RouteId = r.Id;
        ";

        /// <summary>
        /// Obtiene todos los paquetes cuyo peso sea mayor al valor mínimo especificado.
        /// Los resultados se ordenan de forma descendente por peso.
        /// </summary>
        public static string GetHeavyPackages = @"
            SELECT * FROM Packages
            WHERE Weight > @MinWeight
            ORDER BY Weight DESC;
        ";
    }
}
