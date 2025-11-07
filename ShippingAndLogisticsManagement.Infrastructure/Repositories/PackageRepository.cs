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
    public class PackageRepository : BaseRepository<Package>, IPackageRepository
    {
        private readonly IDapperContext _dapper;

        public PackageRepository(LogisticContext context, IDapperContext dapper) : base(context)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<Package>> GetAllAsync(int customerId)
        {
            var sql = @"SELECT p.* FROM Packages p
                        INNER JOIN Shipments s ON p.ShipmentId = s.Id
                        WHERE s.CustomerId = @CustomerId;";

            return await _dapper.QueryAsync<Package>(sql, new { CustomerId = customerId });
        }

        public async Task<IEnumerable<Package>> GetByShipmentIdDapperAsync(int shipmentId)
        {
            return await _dapper.QueryAsync<Package>(
                PackageQueries.GetByShipmentId,
                new { ShipmentId = shipmentId }
            );
        }
        public async Task<IEnumerable<Package>> GetAllDapperAsync(PackageQueryFilter filters)
        {
            var conditions = new List<string>();
            var parameters = new DynamicParameters();

            // Filtro por ShipmentId
            if (filters.ShipmentId.HasValue)
            {
                conditions.Add("ShipmentId = @ShipmentId");
                parameters.Add("ShipmentId", filters.ShipmentId.Value);
            }

            // Filtro por Description (búsqueda parcial)
            if (!string.IsNullOrWhiteSpace(filters.Description))
            {
                conditions.Add("Description LIKE @Description");
                parameters.Add("Description", $"%{filters.Description}%");
            }

            // Filtro por peso mínimo
            if (filters.MinWeight.HasValue)
            {
                conditions.Add("Weight >= @MinWeight");
                parameters.Add("MinWeight", filters.MinWeight.Value);
            }

            // Filtro por peso máximo
            if (filters.MaxWeight.HasValue)
            {
                conditions.Add("Weight <= @MaxWeight");
                parameters.Add("MaxWeight", filters.MaxWeight.Value);
            }

            // Filtro por precio mínimo
            if (filters.MinPrice.HasValue)
            {
                conditions.Add("Price >= @MinPrice");
                parameters.Add("MinPrice", filters.MinPrice.Value);
            }

            // Filtro por precio máximo
            if (filters.MaxPrice.HasValue)
            {
                conditions.Add("Price <= @MaxPrice");
                parameters.Add("MaxPrice", filters.MaxPrice.Value);
            }

            // Cláusula WHERE
            var whereClause = conditions.Any()
                ? "WHERE " + string.Join(" AND ", conditions)
                : string.Empty;

            // Query final
            var sql = $@"
                SELECT * FROM Packages
                {whereClause}
                ORDER BY Id DESC";

            return await _dapper.QueryAsync<Package>(sql, parameters);
        }

        public async Task<Package> GetByIdDapperAsync(int id)
        {
            var sql = "SELECT * FROM Packages WHERE Id = @Id";
            return await _dapper.QueryFirstOrDefaultAsync<Package>(sql, new { Id = id });
        }

        public async Task<PackageSummaryResponse> GetPackageSummaryAsync(int shipmentId)
        {
            return await _dapper.QueryFirstOrDefaultAsync<PackageSummaryResponse>(
                 PackageQueries.GetPackageSummary,
                 new { ShipmentId = shipmentId }
             );
        }

        public async Task<IEnumerable<PackageDetailResponse>> GetPackageWithDetailsAsync()
        {
            return await _dapper.QueryAsync<PackageDetailResponse>(PackageQueries.GetPackageWithDetails);
        }

        public async Task<IEnumerable<Package>> GetHeavyPackagesAsync(double minWeight)
        {
            return await _dapper.QueryAsync<Package>(
                 PackageQueries.GetHeavyPackages,
                 new { MinWeight = minWeight }
             );
        }

        public async Task<IEnumerable<Package>> GetAllDapperAsync(int limit = 10)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => PackageQueries.PackageQuerySqlServer,
                    DatabaseProvider.MySql => PackageQueries.PackageQueryMySQL,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryAsync<Package>(sql, new { Limit = limit });
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
    }
}
