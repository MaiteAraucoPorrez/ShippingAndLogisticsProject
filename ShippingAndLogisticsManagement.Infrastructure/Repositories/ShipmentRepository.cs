using Microsoft.EntityFrameworkCore;
using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Enum;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.Data;
using ShippingAndLogisticsManagement.Infrastructure.Queries;

namespace ShippingAndLogisticsManagement.Infrastructure.Repositories
{
    public class ShipmentRepository : BaseRepository<Shipment>, IShipmentRepository
    {
        private readonly IDapperContext _dapper;
        //private readonly LogisticContext _context;

        public ShipmentRepository(LogisticContext context, IDapperContext dapper) : base(context)
        {
            _dapper = dapper;
            //_context = context;
        }

        public async Task<IEnumerable<Shipment>> GetAllAsync(int customerId)
        {
            var shipments = await _entities.Where(x => x.CustomerId == customerId).ToListAsync();
            return shipments;
        }

        public async Task<IEnumerable<Shipment>> GetAllDapperAsync(int limit = 10)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => ShipmentQueries.ShipmentQuerySqlServer,
                    DatabaseProvider.MySql => ShipmentQueries.ShipmentQueryMySQL,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryAsync<Shipment>(sql, new { Limit = limit });
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<IEnumerable<ShipmentCustomerRouteResponse>> GetShipmentCustomerRouteAsync()
        {
            try
            {
                var sql = ShipmentQueries.InactiveRoutes;

                return await _dapper.QueryAsync<ShipmentCustomerRouteResponse>(sql);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        //public async Task<IEnumerable<Shipment>> GetAllAsync()
        //{
        //    var shipments = await _context.Shipments.ToListAsync();
        //    return shipments;
        //}

        //public async Task<Shipment> GetByIdAsync(int id)
        //{
        //    var shipment = await _context.Shipments.FirstOrDefaultAsync(x => x.Id == id);
        //    return shipment;
        //}

        //public async Task InsertAsync(Shipment shipment)
        //{
        //    _context.Shipments.Add(shipment);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task UpdateAsync(Shipment shipment)
        //{
        //    _context.Shipments.Update(shipment);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task DeleteAsync(Shipment shipment)
        //{
        //    _context.Shipments.Remove(shipment);
        //    await _context.SaveChangesAsync();
        //}
    }
}