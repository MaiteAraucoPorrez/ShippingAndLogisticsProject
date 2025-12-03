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
    public class WarehouseRepository : BaseRepository<Warehouse>, IWarehouseRepository
    {
        private readonly IDapperContext _dapper;

        public WarehouseRepository(LogisticContext context, IDapperContext dapper)
            : base(context)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<Warehouse>> GetAllDapperAsync(WarehouseQueryFilter filters)
        {
            var conditions = new List<string> { "1=1" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(filters.Name))
            {
                conditions.Add("Name LIKE @Name");
                parameters.Add("Name", $"%{filters.Name}%");
            }

            if (!string.IsNullOrWhiteSpace(filters.Code))
            {
                conditions.Add("Code LIKE @Code");
                parameters.Add("Code", $"%{filters.Code}%");
            }

            if (!string.IsNullOrWhiteSpace(filters.City))
            {
                conditions.Add("City LIKE @City");
                parameters.Add("City", $"%{filters.City}%");
            }

            if (!string.IsNullOrWhiteSpace(filters.Department))
            {
                conditions.Add("Department = @Department");
                parameters.Add("Department", filters.Department);
            }

            if (filters.Type.HasValue)
            {
                conditions.Add("Type = @Type");
                parameters.Add("Type", filters.Type.Value.ToString());
            }

            if (filters.IsActive.HasValue)
            {
                conditions.Add("IsActive = @IsActive");
                parameters.Add("IsActive", filters.IsActive.Value);
            }

            if (filters.MinAvailableCapacity.HasValue)
            {
                conditions.Add("(MaxCapacityM3 - CurrentCapacityM3) >= @MinAvailableCapacity");
                parameters.Add("MinAvailableCapacity", filters.MinAvailableCapacity.Value);
            }

            if (filters.MaxOccupancyPercentage.HasValue)
            {
                conditions.Add("(CurrentCapacityM3 / NULLIF(MaxCapacityM3, 0) * 100) <= @MaxOccupancyPercentage");
                parameters.Add("MaxOccupancyPercentage", filters.MaxOccupancyPercentage.Value);
            }

            var whereClause = string.Join(" AND ", conditions);

            var sql = $@"
                SELECT *
                FROM Warehouses
                WHERE {whereClause}
                ORDER BY Type, City, Name;
            ";

            return await _dapper.QueryAsync<Warehouse>(sql, parameters);
        }

        public async Task<Warehouse> GetByIdDapperAsync(int id)
        {
            return await _dapper.QueryFirstOrDefaultAsync<Warehouse>(
                WarehouseQueries.GetById,
                new { Id = id }
            );
        }

        public async Task<Warehouse> GetByCodeAsync(string code)
        {
            return await _dapper.QueryFirstOrDefaultAsync<Warehouse>(
                WarehouseQueries.GetByCode,
                new { Code = code }
            );
        }

        public async Task<IEnumerable<Warehouse>> GetActiveWarehousesAsync()
        {
            return await _dapper.QueryAsync<Warehouse>(
                WarehouseQueries.GetActiveWarehouses
            );
        }

        public async Task<IEnumerable<Warehouse>> GetByCityAsync(string city)
        {
            return await _dapper.QueryAsync<Warehouse>(
                WarehouseQueries.GetByCity,
                new { City = city }
            );
        }

        public async Task<IEnumerable<Warehouse>> GetByDepartmentAsync(string department)
        {
            return await _dapper.QueryAsync<Warehouse>(
                WarehouseQueries.GetByDepartment,
                new { Department = department }
            );
        }

        public async Task<IEnumerable<Warehouse>> GetByTypeAsync(WarehouseType type)
        {
            return await _dapper.QueryAsync<Warehouse>(
                WarehouseQueries.GetByType,
                new { Type = type.ToString() }
            );
        }

        public async Task<IEnumerable<Warehouse>> GetAvailableWarehousesAsync(double requiredCapacity)
        {
            return await _dapper.QueryAsync<Warehouse>(
                WarehouseQueries.GetAvailableWarehouses,
                new { RequiredCapacity = requiredCapacity }
            );
        }

        public async Task<WarehouseStatisticsResponse> GetWarehouseStatisticsAsync(int warehouseId)
        {
            return await _dapper.QueryFirstOrDefaultAsync<WarehouseStatisticsResponse>(
                WarehouseQueries.GetWarehouseStatistics,
                new { WarehouseId = warehouseId }
            );
        }

        public async Task<bool> CodeExistsAsync(string code, int? excludeWarehouseId = null)
        {
            var count = await _dapper.ExecuteScalarAsync<int>(
                WarehouseQueries.CodeExists,
                new { Code = code, ExcludeWarehouseId = excludeWarehouseId }
            );
            return count > 0;
        }

        public async Task<IEnumerable<Warehouse>> GetRecentWarehousesAsync(int limit = 10)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => WarehouseQueries.GetRecentWarehousesSqlServer,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryAsync<Warehouse>(sql, new { Limit = limit });
            }
            catch (Exception err)
            {
                throw new Exception($"Error al obtener almacenes recientes: {err.Message}");
            }
        }
    }
}