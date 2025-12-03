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
    public class VehicleRepository : BaseRepository<Vehicle>, IVehicleRepository
    {
        private readonly IDapperContext _dapper;

        public VehicleRepository(LogisticContext context, IDapperContext dapper)
            : base(context)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<Vehicle>> GetAllDapperAsync(VehicleQueryFilter filters)
        {
            var conditions = new List<string> { "1=1" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(filters.PlateNumber))
            {
                conditions.Add("PlateNumber LIKE @PlateNumber");
                parameters.Add("PlateNumber", $"%{filters.PlateNumber}%");
            }

            if (filters.Type.HasValue)
            {
                conditions.Add("Type = @Type");
                parameters.Add("Type", filters.Type.Value.ToString());
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

            if (filters.BaseWarehouseId.HasValue)
            {
                conditions.Add("BaseWarehouseId = @BaseWarehouseId");
                parameters.Add("BaseWarehouseId", filters.BaseWarehouseId.Value);
            }

            if (filters.AssignedDriverId.HasValue)
            {
                conditions.Add("AssignedDriverId = @AssignedDriverId");
                parameters.Add("AssignedDriverId", filters.AssignedDriverId.Value);
            }

            if (filters.MinAvailableWeightKg.HasValue)
            {
                conditions.Add("(MaxWeightCapacityKg - CurrentWeightKg) >= @MinWeight");
                parameters.Add("MinWeight", filters.MinAvailableWeightKg.Value);
            }

            if (filters.MinAvailableVolumeM3.HasValue)
            {
                conditions.Add("(MaxVolumeCapacityM3 - CurrentVolumeM3) >= @MinVolume");
                parameters.Add("MinVolume", filters.MinAvailableVolumeM3.Value);
            }

            if (filters.RequiresMaintenance.HasValue && filters.RequiresMaintenance.Value)
            {
                conditions.Add(@"(
                    NextMaintenanceDate IS NOT NULL 
                    AND NextMaintenanceDate <= DATEADD(DAY, 7, GETDATE())
                    OR 
                    (LastMaintenanceMileage IS NOT NULL 
                     AND CurrentMileage - LastMaintenanceMileage >= 10000)
                )");
            }

            var whereClause = string.Join(" AND ", conditions);

            var sql = $@"
                SELECT *
                FROM Vehicles
                WHERE {whereClause}
                ORDER BY Type, Brand, Model;
            ";

            return await _dapper.QueryAsync<Vehicle>(sql, parameters);
        }

        public async Task<Vehicle> GetByIdDapperAsync(int id)
        {
            return await _dapper.QueryFirstOrDefaultAsync<Vehicle>(
                VehicleQueries.GetById,
                new { Id = id }
            );
        }

        public async Task<Vehicle> GetByPlateNumberAsync(string plateNumber)
        {
            return await _dapper.QueryFirstOrDefaultAsync<Vehicle>(
                VehicleQueries.GetByPlateNumber,
                new { PlateNumber = plateNumber }
            );
        }

        public async Task<IEnumerable<Vehicle>> GetActiveVehiclesAsync()
        {
            return await _dapper.QueryAsync<Vehicle>(VehicleQueries.GetActiveVehicles);
        }

        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
        {
            return await _dapper.QueryAsync<Vehicle>(VehicleQueries.GetAvailableVehicles);
        }

        public async Task<IEnumerable<Vehicle>> GetByTypeAsync(VehicleType type)
        {
            return await _dapper.QueryAsync<Vehicle>(
                VehicleQueries.GetByType,
                new { Type = type.ToString() }
            );
        }

        public async Task<IEnumerable<Vehicle>> GetByStatusAsync(VehicleStatus status)
        {
            return await _dapper.QueryAsync<Vehicle>(
                VehicleQueries.GetByStatus,
                new { Status = status.ToString() }
            );
        }

        public async Task<IEnumerable<Vehicle>> GetByWarehouseAsync(int warehouseId)
        {
            return await _dapper.QueryAsync<Vehicle>(
                VehicleQueries.GetByWarehouse,
                new { WarehouseId = warehouseId }
            );
        }

        public async Task<IEnumerable<Vehicle>> GetByCapacityAsync(double requiredWeight, double requiredVolume)
        {
            return await _dapper.QueryAsync<Vehicle>(
                VehicleQueries.GetByCapacity,
                new { RequiredWeight = requiredWeight, RequiredVolume = requiredVolume }
            );
        }

        public async Task<VehicleStatisticsResponse> GetVehicleStatisticsAsync(int vehicleId)
        {
            return await _dapper.QueryFirstOrDefaultAsync<VehicleStatisticsResponse>(
                VehicleQueries.GetVehicleStatistics,
                new { VehicleId = vehicleId }
            );
        }

        public async Task<bool> PlateNumberExistsAsync(string plateNumber, int? excludeVehicleId = null)
        {
            var count = await _dapper.ExecuteScalarAsync<int>(
                VehicleQueries.PlateNumberExists,
                new { PlateNumber = plateNumber, ExcludeVehicleId = excludeVehicleId }
            );
            return count > 0;
        }

        public async Task<IEnumerable<Vehicle>> GetRecentVehiclesAsync(int limit = 10)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => VehicleQueries.GetRecentVehiclesSqlServer,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryAsync<Vehicle>(sql, new { Limit = limit });
            }
            catch (Exception err)
            {
                throw new Exception($"Error al obtener vehículos recientes: {err.Message}");
            }
        }
    }
}