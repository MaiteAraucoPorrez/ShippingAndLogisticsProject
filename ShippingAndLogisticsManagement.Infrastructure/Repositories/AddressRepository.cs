using Dapper;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Enum;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.QueryFilters;
using ShippingAndLogisticsManagement.Infrastructure.Data;
using ShippingAndLogisticsManagement.Infrastructure.Queries;

namespace ShippingAndLogisticsManagement.Infrastructure.Repositories
{
    public class AddressRepository : BaseRepository<Address>, IAddressRepository
    {
        private readonly IDapperContext _dapper;

        public AddressRepository(LogisticContext context, IDapperContext dapper)
            : base(context)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<Address>> GetByCustomerIdAsync(int customerId)
        {
            return await _dapper.QueryAsync<Address>(
                AddressQueries.GetByCustomerId,
                new { CustomerId = customerId }
            );
        }

        public async Task<IEnumerable<Address>> GetAllDapperAsync(AddressQueryFilter filters)
        {
            var conditions = new List<string> { "1=1" }; // Siempre verdadero
            var parameters = new DynamicParameters();

            // Filtro por CustomerId
            if (filters.CustomerId.HasValue)
            {
                conditions.Add("CustomerId = @CustomerId");
                parameters.Add("CustomerId", filters.CustomerId.Value);
            }

            // Filtro por City
            if (!string.IsNullOrWhiteSpace(filters.City))
            {
                conditions.Add("City LIKE @City");
                parameters.Add("City", $"%{filters.City}%");
            }

            // Filtro por Department
            if (!string.IsNullOrWhiteSpace(filters.Department))
            {
                conditions.Add("Department = @Department");
                parameters.Add("Department", filters.Department);
            }

            // Filtro por Zone
            if (!string.IsNullOrWhiteSpace(filters.Zone))
            {
                conditions.Add("Zone LIKE @Zone");
                parameters.Add("Zone", $"%{filters.Zone}%");
            }

            // Filtro por Type
            if (filters.Type.HasValue)
            {
                conditions.Add("Type = @Type");
                parameters.Add("Type", filters.Type.Value.ToString());
            }

            // Filtro por IsDefault
            if (filters.IsDefault.HasValue)
            {
                conditions.Add("IsDefault = @IsDefault");
                parameters.Add("IsDefault", filters.IsDefault.Value);
            }

            // Filtro por IsActive
            if (filters.IsActive.HasValue)
            {
                conditions.Add("IsActive = @IsActive");
                parameters.Add("IsActive", filters.IsActive.Value);
            }

            // Búsqueda por texto en Street o Reference
            if (!string.IsNullOrWhiteSpace(filters.SearchText))
            {
                conditions.Add("(Street LIKE @SearchText OR Reference LIKE @SearchText)");
                parameters.Add("SearchText", $"%{filters.SearchText}%");
            }

            var whereClause = string.Join(" AND ", conditions);

            var sql = $@"
                SELECT *
                FROM Addresses
                WHERE {whereClause}
                ORDER BY IsDefault DESC, CreatedAt DESC;
            ";

            return await _dapper.QueryAsync<Address>(sql, parameters);
        }

        public async Task<Address> GetByIdDapperAsync(int id)
        {
            return await _dapper.QueryFirstOrDefaultAsync<Address>(
                AddressQueries.GetById,
                new { Id = id }
            );
        }

        public async Task<Address> GetDefaultAddressAsync(int customerId, AddressType type)
        {
            return await _dapper.QueryFirstOrDefaultAsync<Address>(
                AddressQueries.GetDefaultAddress,
                new { CustomerId = customerId, Type = type.ToString() }
            );
        }

        public async Task<IEnumerable<Address>> GetByCityAsync(string city)
        {
            return await _dapper.QueryAsync<Address>(
                AddressQueries.GetByCity,
                new { City = city }
            );
        }

        public async Task<IEnumerable<Address>> GetByDepartmentAsync(string department)
        {
            return await _dapper.QueryAsync<Address>(
                AddressQueries.GetByDepartment,
                new { Department = department }
            );
        }

        public async Task<bool> HasDefaultAddressAsync(int customerId, AddressType type, int? excludeAddressId = null)
        {
            var count = await _dapper.ExecuteScalarAsync<int>(
                AddressQueries.HasDefaultAddress,
                new
                {
                    CustomerId = customerId,
                    Type = type.ToString(),
                    ExcludeAddressId = excludeAddressId
                }
            );
            return count > 0;
        }

        public async Task UnsetDefaultAddressesAsync(int customerId, AddressType type, int? excludeAddressId = null)
        {
            await _dapper.ExecuteAsync(
                AddressQueries.UnsetDefaultAddresses,
                new
                {
                    CustomerId = customerId,
                    Type = type.ToString(),
                    ExcludeAddressId = excludeAddressId
                }
            );
        }

        public async Task<int> CountActiveAddressesAsync(int customerId)
        {
            return await _dapper.ExecuteScalarAsync<int>(
                AddressQueries.CountActiveAddresses,
                new { CustomerId = customerId }
            );
        }

        public async Task<IEnumerable<Address>> GetRecentAddressesAsync(int limit = 10)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => AddressQueries.GetRecentAddressesSqlServer,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryAsync<Address>(sql, new { Limit = limit });
            }
            catch (Exception err)
            {
                throw new Exception($"Error al obtener direcciones recientes: {err.Message}");
            }
        }
    }
}