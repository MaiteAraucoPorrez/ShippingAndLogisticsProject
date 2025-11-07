using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    public interface IShipmentService
    {
        Task<ResponseData> GetAllAsync(ShipmentQueryFilter shipmentQueryFilter);
        Task<IEnumerable<Shipment>> GetAllDapperAsync();
        Task<IEnumerable<ShipmentCustomerRouteResponse>> GetShipmentCustomerRouteAsync();
        Task<Shipment> GetByIdAsync(int id);
        Task InsertAsync(Shipment shipment);
        Task UpdateAsync(Shipment shipment);
        Task DeleteAsync(int id);
    }
}
