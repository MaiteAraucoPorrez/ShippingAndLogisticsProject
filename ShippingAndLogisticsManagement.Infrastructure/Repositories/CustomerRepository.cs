using Dapper;
using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Enum;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.QueryFilters;
using ShippingAndLogisticsManagement.Infrastructure.Data;
using ShippingAndLogisticsManagement.Infrastructure.Queries;

namespace ShippingAndLogisticsManagement.Infrastructure.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        private readonly IDapperContext _dapper;

        public CustomerRepository(LogisticContext context, IDapperContext dapper) : base(context)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<Customer>> GetAllDapperAsync(CustomerQueryFilter filters)
        {
            var conditions = new List<string>();
            var parameters = new DynamicParameters();

            // Filter by Name (partial search)
            if (!string.IsNullOrWhiteSpace(filters.Name))
            {
                conditions.Add("Name LIKE @Name");
                parameters.Add("Name", $"%{filters.Name}%");
            }

            // Filter by Email (partial search)
            if (!string.IsNullOrWhiteSpace(filters.Email))
            {
                conditions.Add("Email LIKE @Email");
                parameters.Add("Email", $"%{filters.Email}%");
            }

            // Filter by Phone (partial search)
            if (!string.IsNullOrWhiteSpace(filters.Phone))
            {
                conditions.Add("Phone LIKE @Phone");
                parameters.Add("Phone", $"%{filters.Phone}%");
            }

            // WHERE clause
            var whereClause = conditions.Any()
                ? "WHERE " + string.Join(" AND ", conditions)
                : string.Empty;

            // Final query
            var sql = $@"
                SELECT * FROM Customers
                {whereClause}
                ORDER BY Id DESC";

            return await _dapper.QueryAsync<Customer>(sql, parameters);
        }

        public async Task<Customer> GetByIdDapperAsync(int id)
        {
            var sql = "SELECT * FROM Customers WHERE Id = @Id";
            return await _dapper.QueryFirstOrDefaultAsync<Customer>(sql, new { Id = id });
        }

        public async Task<Customer> GetByEmailAsync(string email)
        {
            return await _dapper.QueryFirstOrDefaultAsync<Customer>(
                CustomerQueries.GetByEmail,
                new { Email = email }
            );
        }

        public async Task<IEnumerable<CustomerShipmentHistoryResponse>> GetCustomerShipmentHistoryAsync(int customerId)
        {
            return await _dapper.QueryAsync<CustomerShipmentHistoryResponse>(
                CustomerQueries.GetCustomerShipmentHistory,
                new { CustomerId = customerId }
            );
        }

        public async Task<int> CountByEmailDomainAsync(string emailDomain)
        {
            return await _dapper.ExecuteScalarAsync<int>(
                CustomerQueries.CountByEmailDomain,
                new { EmailDomain = $"%@{emailDomain}" }
            );
        }

        public async Task<bool> HasActiveShipmentsAsync(int customerId)
        {
            var count = await _dapper.ExecuteScalarAsync<int>(
                CustomerQueries.CountActiveShipments,
                new { CustomerId = customerId }
            );
            return count > 0;
        }

        public async Task<IEnumerable<Customer>> GetRecentCustomersAsync(int limit = 10)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => CustomerQueries.CustomerQuerySqlServer,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryAsync<Customer>(sql, new { Limit = limit });
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
    }
}
