using Dapper;
using Microsoft.EntityFrameworkCore;
using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Enum;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.QueryFilters;
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

        public async Task<IEnumerable<Shipment>> GetRecentShipmentsAsync(int limit = 10)
        {
            try
            {
                var sql = _dapper.Provider switch
                {
                    DatabaseProvider.SqlServer => ShipmentQueries.ShipmentQuerySqlServer,
                    _ => throw new NotSupportedException("Provider no soportado")
                };

                return await _dapper.QueryAsync<Shipment>(sql, new { Limit = limit });
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<IEnumerable<Shipment>> GetAllDapperAsync(ShipmentQueryFilter filters)
        {
            var conditions = new List<string>();
            var parameters = new DynamicParameters();

            if (filters.CustomerId.HasValue)
            {
                conditions.Add("CustomerId = @CustomerId");
                parameters.Add("CustomerId", filters.CustomerId.Value);
            }

            if (filters.ShippingDate.HasValue)
            {
                conditions.Add("CAST(ShippingDate AS DATE) = @ShippingDate");
                parameters.Add("ShippingDate", filters.ShippingDate.Value.Date);
            }

            if (!string.IsNullOrWhiteSpace(filters.State))
            {
                conditions.Add("State LIKE @State");
                parameters.Add("State", $"%{filters.State}%");
            }

            if (filters.RouteId.HasValue)
            {
                conditions.Add("RouteId = @RouteId");
                parameters.Add("RouteId", filters.RouteId.Value);
            }

            if (filters.TotalCost > 0)
            {
                conditions.Add("TotalCost = @TotalCost");
                parameters.Add("TotalCost", filters.TotalCost);
            }

            if (!string.IsNullOrWhiteSpace(filters.TrackingNumber))
            {
                conditions.Add("TrackingNumber = @TrackingNumber");
                parameters.Add("TrackingNumber", filters.TrackingNumber);
            }

            var whereClause = conditions.Any()
                ? "WHERE " + string.Join(" AND ", conditions)
                : string.Empty;

            var sql = $@"
            SELECT * FROM Shipments
            {whereClause}
            ORDER BY ShippingDate DESC";

            return await _dapper.QueryAsync<Shipment>(sql, parameters);
        }

        public async Task<Shipment> GetByIdDapperAsync(int id)
        {
            var sql = "SELECT * FROM Shipments WHERE Id = @Id";
            return await _dapper.QueryFirstOrDefaultAsync<Shipment>(sql, new { Id = id });
        }

        public async Task<IEnumerable<ShipmentCustomerRouteResponse>> GetShipmentCustomerRouteAsync()
        {
            try
            {
                var sql = ShipmentQueries.GetShipmentCustomerRoute;

                return await _dapper.QueryAsync<ShipmentCustomerRouteResponse>(sql);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public async Task<IEnumerable<ShipmentReportResponse>> GetReportByDateAndState(DateTime startDate, DateTime endDate)
        {
            var sql = ShipmentQueries.GetShipmentsByDateRangeAndState;
            var parameters = new { StartDate = startDate, EndDate = endDate };

            var result = await _dapper.QueryAsync<ShipmentReportResponse>(sql, parameters);
            foreach (var item in result)
            {
                item.DayName = item.Date.DayOfWeek.ToString();
            }

            return result;
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