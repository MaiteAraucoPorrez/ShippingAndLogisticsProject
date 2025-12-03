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
    public class RouteRepository : BaseRepository<Route>, IRouteRepository
    {
        private readonly IDapperContext _dapper;

        public RouteRepository(LogisticContext context, IDapperContext dapper) : base(context)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<Route>> GetAllDapperAsync(RouteQueryFilter filters)
        {
            var conditions = new List<string>();
            var parameters = new DynamicParameters();

            // Filter by Origin (partial search)
            if (!string.IsNullOrWhiteSpace(filters.Origin))
            {
                conditions.Add("Origin LIKE @Origin");
                parameters.Add("Origin", $"%{filters.Origin}%");
            }

            // Filter by Destination (partial search)
            if (!string.IsNullOrWhiteSpace(filters.Destination))
            {
                conditions.Add("Destination LIKE @Destination");
                parameters.Add("Destination", $"%{filters.Destination}%");
            }

            // Filter by minimum distance
            if (filters.MinDistance.HasValue)
            {
                conditions.Add("DistanceKm >= @MinDistance");
                parameters.Add("MinDistance", filters.MinDistance.Value);
            }

            // Filter by maximum distance
            if (filters.MaxDistance.HasValue)
            {
                conditions.Add("DistanceKm <= @MaxDistance");
                parameters.Add("MaxDistance", filters.MaxDistance.Value);
            }

            // Filter by minimum cost
            if (filters.MinCost.HasValue)
            {
                conditions.Add("BaseCost >= @MinCost");
                parameters.Add("MinCost", filters.MinCost.Value);
            }

            // Filter by maximum cost
            if (filters.MaxCost.HasValue)
            {
                conditions.Add("BaseCost <= @MaxCost");
                parameters.Add("MaxCost", filters.MaxCost.Value);
            }

            // Filter by active status
            if (filters.IsActive.HasValue)
            {
                conditions.Add("IsActive = @IsActive");
                parameters.Add("IsActive", filters.IsActive.Value);
            }

            // WHERE clause
            var whereClause = conditions.Any()
                ? "WHERE " + string.Join(" AND ", conditions)
                : string.Empty;

            // Final query
            var sql = $@"
                SELECT * FROM Routes
                {whereClause}
                ORDER BY Id DESC";

            return await _dapper.QueryAsync<Route>(sql, parameters);
        }

        public async Task<Route> GetByIdDapperAsync(int id)
        {
            var sql = "SELECT * FROM Routes WHERE Id = @Id";
            return await _dapper.QueryFirstOrDefaultAsync<Route>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Route>> GetActiveRoutesAsync()
        {
            return await _dapper.QueryAsync<Route>(
                RouteQueries.GetActiveRoutes,
                new { }
            );
        }

        public async Task<IEnumerable<RouteRankingResponse>> GetMostUsedRoutesAsync(int limit = 10)
        {
            var ranking = await _dapper.QueryAsync<RouteRankingResponse>(
                RouteQueries.GetMostUsedRoutes,
                new { Limit = limit }
            );

            // Add rank position
            int position = 1;
            foreach (var route in ranking)
            {
                route.Rank = position++;
            }

            return ranking;
        }

        public async Task<Route> GetByOriginDestinationAsync(string origin, string destination)
        {
            return await _dapper.QueryFirstOrDefaultAsync<Route>(
                RouteQueries.GetByOriginDestination,
                new { Origin = origin, Destination = destination }
            );
        }

        public async Task<bool> HasShipmentsAsync(int routeId)
        {
            var count = await _dapper.ExecuteScalarAsync<int>(
                RouteQueries.CountShipmentsByRoute,
                new { RouteId = routeId }
            );
            return count > 0;
        }

        public async Task<IEnumerable<Route>> GetRecentRoutesAsync(int limit = 10)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => RouteQueries.RouteQuerySqlServer,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryAsync<Route>(sql, new { Limit = limit });
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
    }
}
