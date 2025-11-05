using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;
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
            //_context = context;
        }

        public async Task<IEnumerable<Package>> GetAllAsync(int customerId)
        {
            var sql = @"SELECT p.* FROM Package p
                        INNER JOIN Shipment s ON p.ShipmentId = s.Id
                        WHERE s.CustomerId = @CustomerId;";

            return await _dapper.QueryAsync<Package>(sql, new { CustomerId = customerId });
        }

        public async Task<IEnumerable<Package>> GetByShipmentIdDapperAsync(int shipmentId)
        {
            return await _dapper.QueryAsync<Package>(PackageQueries.GetByShipmentId, new { ShipmentId = shipmentId });
        }

        public async Task<PackageSummaryResponse> GetPackageSummaryAsync(int shipmentId)
        {
            return await _dapper.QueryFirstOrDefaultAsync<PackageSummaryResponse>(PackageQueries.GetPackageSummary, new { ShipmentId = shipmentId });
        }

        public async Task<IEnumerable<Package>> GetAllDapperAsync(int limit = 10)
        {
            return await _dapper.QueryAsync<Package>(PackageQueries.GetAllDapper, new { Limit = limit });
        }

        public async Task<Package> GetByIdDapperAsync(int id)
        {
            return await _dapper.QueryFirstOrDefaultAsync<Package>(PackageQueries.GetByIdDapper, new { Id = id });
        }

        public async Task<IEnumerable<PackageDetailResponse>> GetPackageWithDetailsAsync()
        {
            return await _dapper.QueryAsync<PackageDetailResponse>(PackageQueries.GetPackageWithDetails);
        }

        public async Task<IEnumerable<Package>> GetHeavyPackagesAsync(double minWeight)
        {
            return await _dapper.QueryAsync<Package>(PackageQueries.GetHeavyPackages, new { MinWeight = minWeight });
        }
    }
}
