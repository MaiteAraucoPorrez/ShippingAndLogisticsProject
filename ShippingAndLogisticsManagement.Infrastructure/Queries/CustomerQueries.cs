namespace ShippingAndLogisticsManagement.Infrastructure.Queries
{
    /// <summary>
    /// Contiene todas las consultas SQL relacionadas con la entidad <see cref="Customer"/>.
    /// Incluye versiones específicas para SQL Server y MySQL, así como consultas
    /// especializadas para reportes, historial y operaciones con Dapper.
    /// </summary>
    public static class CustomerQueries
    {
        /// <summary>
        /// Consulta para SQL Server:
        /// Obtiene una lista de clientes ordenados por ID descendente,
        /// utilizando paginación con OFFSET-FETCH.
        /// </summary>
        public static string CustomerQuerySqlServer = @"
            SELECT 
                c.Id,
                c.Name,
                c.Email,
                c.Phone
            FROM Customers c
            ORDER BY c.Id DESC
            OFFSET 0 ROWS FETCH NEXT @Limit ROWS ONLY;
        ";

        /// <summary>
        /// Consulta para MySQL:
        /// Devuelve los clientes ordenados por ID descendente,
        /// limitando la cantidad de resultados con LIMIT.
        /// </summary>
        public static string CustomerQueryMySQL = @"
            SELECT 
                c.Id,
                c.Name,
                c.Email,
                c.Phone
            FROM Customers c
            ORDER BY c.Id DESC
            LIMIT @Limit;
        ";

        /// <summary>
        /// Obtiene un cliente por su dirección de correo electrónico.
        /// </summary>
        public static string GetByEmail = @"
            SELECT * FROM Customers
            WHERE Email = @Email;
        ";

        /// <summary>
        /// Obtiene el historial completo de envíos de un cliente con información detallada.
        /// Incluye: envío, ruta, cantidad de paquetes, peso total y valor total.
        /// </summary>
        public static string GetCustomerShipmentHistory = @"
            SELECT 
                s.Id AS ShipmentId,
                s.TrackingNumber,
                s.ShippingDate,
                s.State,
                s.TotalCost,
                r.Origin AS RouteOrigin,
                r.Destination AS RouteDestination,
                r.DistanceKm AS RouteDistanceKm,
                COUNT(p.Id) AS PackageCount,
                ISNULL(SUM(p.Weight), 0) AS TotalWeight,
                ISNULL(SUM(p.Price), 0) AS TotalValue
            FROM Shipments s
            INNER JOIN Routes r ON s.RouteId = r.Id
            LEFT JOIN Packages p ON s.Id = p.ShipmentId
            WHERE s.CustomerId = @CustomerId
            GROUP BY s.Id, s.TrackingNumber, s.ShippingDate, s.State, s.TotalCost, 
                     r.Origin, r.Destination, r.DistanceKm
            ORDER BY s.ShippingDate DESC;
        ";

        /// <summary>
        /// Cuenta cuántos clientes existen con un dominio de email específico.
        /// </summary>
        public static string CountByEmailDomain = @"
            SELECT COUNT(*) 
            FROM Customers
            WHERE Email LIKE @EmailDomain;
        ";

        /// <summary>
        /// Cuenta cuántos envíos activos (no entregados) tiene un cliente.
        /// </summary>
        public static string CountActiveShipments = @"
            SELECT COUNT(*) 
            FROM Shipments
            WHERE CustomerId = @CustomerId 
            AND State != 'Delivered';
        ";

        /// <summary>
        /// Obtiene todos los clientes con su cantidad de envíos activos.
        /// Útil para reportes y análisis de clientes.
        /// </summary>
        public static string GetCustomersWithActiveShipmentsCount = @"
            SELECT 
                c.Id,
                c.Name,
                c.Email,
                c.Phone,
                COUNT(s.Id) AS ActiveShipmentsCount
            FROM Customers c
            LEFT JOIN Shipments s ON c.Id = s.CustomerId AND s.State != 'Delivered'
            GROUP BY c.Id, c.Name, c.Email, c.Phone
            ORDER BY ActiveShipmentsCount DESC;
        ";

        /// <summary>
        /// Obtiene clientes que NO tienen ningún envío registrado.
        /// Útil para campañas de marketing o seguimiento de clientes inactivos.
        /// </summary>
        public static string GetCustomersWithoutShipments = @"
            SELECT c.*
            FROM Customers c
            LEFT JOIN Shipments s ON c.Id = s.CustomerId
            WHERE s.Id IS NULL;
        ";
    }
}
