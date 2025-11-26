namespace ShippingAndLogisticsManagement.Infrastructure.Queries
{
    /// <summary>
    /// Consultas SQL optimizadas para la entidad Address
    /// </summary>
    public static class AddressQueries
    {
        /// <summary>
        /// Obtiene direcciones recientes (SQL Server)
        /// </summary>
        public static string GetRecentAddressesSqlServer = @"
            SELECT TOP (@Limit)
                a.*
            FROM Addresses a
            ORDER BY a.CreatedAt DESC, a.Id DESC;
        ";

        /// <summary>
        /// Obtiene direcciones recientes (MySQL)
        /// </summary>
        public static string GetRecentAddressesMySQL = @"
            SELECT a.*
            FROM Addresses a
            ORDER BY a.CreatedAt DESC, a.Id DESC
            LIMIT @Limit;
        ";

        /// <summary>
        /// Obtiene todas las direcciones de un cliente
        /// </summary>
        public static string GetByCustomerId = @"
            SELECT *
            FROM Addresses
            WHERE CustomerId = @CustomerId
            AND IsActive = 1
            ORDER BY IsDefault DESC, CreatedAt DESC;
        ";

        /// <summary>
        /// Obtiene una dirección por ID
        /// </summary>
        public static string GetById = @"
            SELECT *
            FROM Addresses
            WHERE Id = @Id;
        ";

        /// <summary>
        /// Obtiene la dirección predeterminada de un cliente por tipo
        /// </summary>
        public static string GetDefaultAddress = @"
            SELECT TOP 1 *
            FROM Addresses
            WHERE CustomerId = @CustomerId
            AND Type = @Type
            AND IsDefault = 1
            AND IsActive = 1;
        ";

        /// <summary>
        /// Obtiene direcciones por ciudad
        /// </summary>
        public static string GetByCity = @"
            SELECT *
            FROM Addresses
            WHERE City LIKE '%' + @City + '%'
            AND IsActive = 1
            ORDER BY City, CreatedAt DESC;
        ";

        /// <summary>
        /// Obtiene direcciones por departamento
        /// </summary>
        public static string GetByDepartment = @"
            SELECT *
            FROM Addresses
            WHERE Department = @Department
            AND IsActive = 1
            ORDER BY City, CreatedAt DESC;
        ";

        /// <summary>
        /// Verifica si existe una dirección predeterminada
        /// </summary>
        public static string HasDefaultAddress = @"
            SELECT COUNT(*)
            FROM Addresses
            WHERE CustomerId = @CustomerId
            AND Type = @Type
            AND IsDefault = 1
            AND IsActive = 1
            AND (@ExcludeAddressId IS NULL OR Id != @ExcludeAddressId);
        ";

        /// <summary>
        /// Desmarca direcciones predeterminadas
        /// </summary>
        public static string UnsetDefaultAddresses = @"
            UPDATE Addresses
            SET IsDefault = 0
            WHERE CustomerId = @CustomerId
            AND Type = @Type
            AND (@ExcludeAddressId IS NULL OR Id != @ExcludeAddressId);
        ";

        /// <summary>
        /// Cuenta direcciones activas de un cliente
        /// </summary>
        public static string CountActiveAddresses = @"
            SELECT COUNT(*)
            FROM Addresses
            WHERE CustomerId = @CustomerId
            AND IsActive = 1;
        ";

        /// <summary>
        /// Obtiene estadísticas de direcciones por departamento
        /// </summary>
        public static string GetAddressStatsByDepartment = @"
            SELECT 
                Department,
                COUNT(*) AS TotalAddresses,
                SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) AS ActiveAddresses,
                SUM(CASE WHEN IsDefault = 1 THEN 1 ELSE 0 END) AS DefaultAddresses
            FROM Addresses
            GROUP BY Department
            ORDER BY TotalAddresses DESC;
        ";
    }
}