using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using ShippingAndLogisticsManagement.Core.Enum;
using ShippingAndLogisticsManagement.Core.Interfaces;
using System.Data;


namespace ShippingAndLogisticsManagement.Infrastructure.Repositories
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _sqlConn;
        private readonly string _mySqlConn;
        private readonly IConfiguration _config;

        public DbConnectionFactory(IConfiguration config)
        {
            _config = config;
            _sqlConn = _config.GetConnectionString("ConnectionSqlServer") ?? string.Empty;
            _mySqlConn = _config.GetConnectionString("ConnectionMySql") ?? string.Empty;

            var providerStr = _config.GetSection("DatabaseProvider").Value
                ?? "SqlServer";

            Provider = providerStr.Equals("SqlServer", 
                StringComparison.OrdinalIgnoreCase)
                ? DatabaseProvider.SqlServer
                : DatabaseProvider.MySql;

        }
        public DatabaseProvider Provider { get; }

        public IDbConnection CreateConnection()
        {
            return Provider switch
            {
                DatabaseProvider.SqlServer => new SqlConnection(_sqlConn),
                _ => new MySqlConnection(_mySqlConn)
            };
        }
    }
}