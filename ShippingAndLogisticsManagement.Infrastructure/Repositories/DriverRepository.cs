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
    public class DriverRepository : BaseRepository<Driver>, IDriverRepository
    {
        private readonly IDapperContext _dapper;

        public DriverRepository(LogisticContext context, IDapperContext dapper)
            : base(context)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<Driver>> GetAllDapperAsync(DriverQueryFilter filters)
        {
            var conditions = new List<string> { "1=1" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(filters.FullName))
            {
                conditions.Add("FullName LIKE @FullName");
                parameters.Add("FullName", $"%{filters.FullName}%");
            }

            if (!string.IsNullOrWhiteSpace(filters.LicenseNumber))
            {
                conditions.Add("LicenseNumber LIKE @LicenseNumber");
                parameters.Add("LicenseNumber", $"%{filters.LicenseNumber}%");
            }

            if (filters.Status.HasValue)
            {
                conditions.Add("Status = @Status");
                parameters.Add("Status", filters.Status.Value.ToString());
            }

            if (filters.IsActive.HasValue)
            {
                conditions.Add("IsActive = @IsActive");
                parameters.Add("IsActive", filters.IsActive.Value);
            }

            if (filters.CurrentVehicleId.HasValue)
            {
                conditions.Add("CurrentVehicleId = @CurrentVehicleId");
                parameters.Add("CurrentVehicleId", filters.CurrentVehicleId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filters.Email))
            {
                conditions.Add("Email LIKE @Email");
                parameters.Add("Email", $"%{filters.Email}%");
            }

            if (!string.IsNullOrWhiteSpace(filters.Phone))
            {
                conditions.Add("Phone LIKE @Phone");
                parameters.Add("Phone", $"%{filters.Phone}%");
            }

            var whereClause = string.Join(" AND ", conditions);

            var sql = $@"
                SELECT *
                FROM Drivers
                WHERE {whereClause}
                ORDER BY FullName;
            ";

            return await _dapper.QueryAsync<Driver>(sql, parameters);
        }

        public async Task<Driver> GetByIdDapperAsync(int id)
        {
            return await _dapper.QueryFirstOrDefaultAsync<Driver>(
                DriverQueries.GetById,
                new { Id = id }
            );
        }

        public async Task<Driver> GetByLicenseNumberAsync(string licenseNumber)
        {
            return await _dapper.QueryFirstOrDefaultAsync<Driver>(
                DriverQueries.GetByLicenseNumber,
                new { LicenseNumber = licenseNumber }
            );
        }

        public async Task<IEnumerable<Driver>> GetActiveDriversAsync()
        {
            return await _dapper.QueryAsync<Driver>(DriverQueries.GetActiveDrivers);
        }

        public async Task<IEnumerable<Driver>> GetAvailableDriversAsync()
        {
            return await _dapper.QueryAsync<Driver>(DriverQueries.GetAvailableDrivers);
        }

        public async Task<IEnumerable<Driver>> GetByStatusAsync(DriverStatus status)
        {
            return await _dapper.QueryAsync<Driver>(
                DriverQueries.GetByStatus,
                new { Status = status.ToString() }
            );
        }

        public async Task<DriverStatisticsResponse> GetDriverStatisticsAsync(int driverId)
        {
            return await _dapper.QueryFirstOrDefaultAsync<DriverStatisticsResponse>(
                DriverQueries.GetDriverStatistics,
                new { DriverId = driverId }
            );
        }

        public async Task<IEnumerable<Driver>> GetRecentDriversAsync(int limit = 10)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => DriverQueries.GetRecentDriversSqlServer,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryAsync<Driver>(sql, new { Limit = limit });
            }
            catch (Exception err)
            {
                throw new Exception($"Error al obtener conductores recientes: {err.Message}");
            }
        }
    }
}