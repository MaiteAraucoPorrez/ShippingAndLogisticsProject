using Dapper;
using ShippingAndLogisticsManagement.Core.Enum;
using ShippingAndLogisticsManagement.Core.Interfaces;
using System.Data;
using System.Data.Common;

namespace ShippingAndLogisticsManagement.Infrastructure.Data
{
    public class DapperContext : IDapperContext
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private static readonly AsyncLocal<(IDbConnection? Conn, IDbTransaction? Tx)>
         _ambientContext = new();

        public DapperContext(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public DatabaseProvider Provider => _dbConnectionFactory.Provider;

        public void ClearAmbientConnection()
        {
            _ambientContext.Value = (null, null);
        }

        private (IDbConnection conn, IDbTransaction? tx, bool ownsConnection) GetConnAndTx()
        {
            var ambient = _ambientContext.Value;
            if (ambient.Conn != null)
            {
                return (ambient.Conn, ambient.Tx, false);
            }

            var conn = _dbConnectionFactory.CreateConnection();
            return (conn, null, true);
        }

        public void SetAmbientConnection(IDbConnection conn, IDbTransaction? tx)
        {
            _ambientContext.Value = (conn, tx);
        }

        public async Task OpenIfNeedAsync(IDbConnection conn)
        {
            if (conn is DbConnection dbConn && dbConn.State == ConnectionState.Closed)
            {
                await dbConn.OpenAsync();
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null,
     CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();

            try
            {
                await OpenIfNeedAsync(conn);
                return await conn.QueryAsync<T>
                    (new CommandDefinition(sql, param, tx, commandType: commandType));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (owns)
                {
                    if (conn is DbConnection dbConn && dbConn.State != ConnectionState.Closed)
                    {
                        await dbConn.CloseAsync();
                        conn.Dispose();
                    }
                }
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null,
            CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();

            try
            {
                await OpenIfNeedAsync(conn);
                return await conn.QueryFirstOrDefaultAsync<T>
                    (new CommandDefinition(sql, param, tx, commandType: commandType));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (owns)
                {
                    if (conn is DbConnection dbConn && dbConn.State != ConnectionState.Closed)
                    {
                        await dbConn.CloseAsync();
                        conn.Dispose();
                    }
                }
            }
        }

        public async Task<int> ExecuteAsync(string sql, object? param = null,
            CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();

            try
            {
                await OpenIfNeedAsync(conn);
                return await conn.ExecuteAsync
                    (new CommandDefinition(sql, param, tx, commandType: commandType));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (owns)
                {
                    if (conn is DbConnection dbConn && dbConn.State != ConnectionState.Closed)
                    {
                        await dbConn.CloseAsync();
                        conn.Dispose();
                    }
                }
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, object? param = null,
            CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();

            try
            {
                await OpenIfNeedAsync(conn);
                var res = await conn.ExecuteScalarAsync<T>
                    (new CommandDefinition(sql, param, tx, commandType: commandType));

                if (res == null || (res is DBNull)) return default!;
                return (T)Convert.ChangeType(res, typeof(T));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (owns)
                {
                    if (conn is DbConnection dbConn && dbConn.State != ConnectionState.Closed)
                    {
                        await dbConn.CloseAsync();
                        conn.Dispose();
                    }
                }
            }
        } 
    }
}