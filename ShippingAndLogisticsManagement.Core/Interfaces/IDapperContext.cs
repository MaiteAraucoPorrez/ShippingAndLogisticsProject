using ShippingAndLogisticsManagement.Core.Enum;
using System.Data;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Defines an abstraction for executing SQL queries and commands using Dapper with support for ambient database
    /// connections and transactions.
    /// </summary>
    /// <remarks>This interface provides methods for querying and executing SQL statements asynchronously, as
    /// well as managing the ambient database connection and transaction context. Implementations are expected to handle
    /// connection management and command execution in a way that is compatible with Dapper. The ambient connection
    /// mechanism allows callers to set a specific connection and transaction to be used for subsequent operations,
    /// which is useful for scenarios such as unit of work or explicit transaction management.</remarks>
    public interface IDapperContext
    {
        DatabaseProvider Provider { get; }

        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null,
        CommandType commandType = CommandType.Text);

        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null,
            CommandType commandType = CommandType.Text);

        Task<int> ExecuteAsync(string sql, object? param = null,
            CommandType commandType = CommandType.Text);

        Task<T> ExecuteScalarAsync<T>(string sql, object? param = null,
            CommandType commandType = CommandType.Text);

        void SetAmbientConnection(IDbConnection conn, IDbTransaction? transaction);

        void ClearAmbientConnection();
    }
}