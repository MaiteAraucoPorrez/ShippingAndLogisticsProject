using ShippingAndLogisticsManagement.Core.Enum;
using System.Data;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Defines a factory for creating database connections using a specified database provider.
    /// </summary>
    /// <remarks>Implementations of this interface are responsible for supplying connections compatible with
    /// the configured provider. The returned connections are typically not open; callers should open and manage the
    /// connection lifecycle as needed.</remarks>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Gets the database provider used by the current context.
        /// </summary>
        DatabaseProvider Provider { get; }

        /// <summary>
        /// Creates and returns a new database connection instance.
        /// </summary>
        /// <remarks>The caller is responsible for opening and disposing the returned connection. The
        /// specific implementation of <see cref="IDbConnection"/> depends on the underlying database
        /// provider.</remarks>
        /// <returns>An <see cref="IDbConnection"/> representing the newly created database connection. The connection is not
        /// opened by default.</returns>
        IDbConnection CreateConnection();
    }
}
