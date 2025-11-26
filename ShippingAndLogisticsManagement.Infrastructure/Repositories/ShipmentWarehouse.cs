using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.Data;
using ShippingAndLogisticsManagement.Infrastructure.Queries;

namespace ShippingAndLogisticsManagement.Infrastructure.Repositories
{
    public class ShipmentWarehouseRepository : BaseRepository<ShipmentWarehouse>, IShipmentWarehouseRepository
    {
        private readonly IDapperContext _dapper;

        public ShipmentWarehouseRepository(LogisticContext context, IDapperContext dapper)
            : base(context)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<ShipmentWarehouseHistoryResponse>> GetShipmentHistoryAsync(int shipmentId)
        {
            return await _dapper.QueryAsync<ShipmentWarehouseHistoryResponse>(
                WarehouseQueries.GetShipmentHistory,
                new { ShipmentId = shipmentId }
            );
        }

        public async Task<IEnumerable<ShipmentWarehouse>> GetCurrentShipmentsInWarehouseAsync(int warehouseId)
        {
            return await _dapper.QueryAsync<ShipmentWarehouse>(
                WarehouseQueries.GetCurrentShipmentsInWarehouse,
                new { WarehouseId = warehouseId }
            );
        }

        public async Task<ShipmentWarehouse> GetCurrentWarehouseForShipmentAsync(int shipmentId)
        {
            return await _dapper.QueryFirstOrDefaultAsync<ShipmentWarehouse>(
                WarehouseQueries.GetCurrentWarehouseForShipment,
                new { ShipmentId = shipmentId }
            );
        }

        public async Task RegisterEntryAsync(ShipmentWarehouse shipmentWarehouse)
        {
            await Add(shipmentWarehouse);
        }

        public async Task RegisterExitAsync(int shipmentWarehouseId, DateTime exitDate, string dispatchedBy)
        {
            var existing = await GetById(shipmentWarehouseId);
            if (existing != null)
            {
                existing.ExitDate = exitDate;
                existing.DispatchedBy = dispatchedBy;
                existing.Status = WarehouseShipmentStatus.Dispatched;
                await Update(existing);
            }
        }

        public async Task<bool> IsShipmentInWarehouseAsync(int shipmentId)
        {
            var count = await _dapper.ExecuteScalarAsync<int>(
                WarehouseQueries.IsShipmentInWarehouse,
                new { ShipmentId = shipmentId }
            );
            return count > 0;
        }
    }
}