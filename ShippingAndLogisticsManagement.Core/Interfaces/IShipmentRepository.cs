using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    public interface IShipmentRepository: IBaseRepository<Shipment>
    {
        Task<IEnumerable<Shipment>> GetAllAsync(int customerId);
        Task<IEnumerable<Shipment>> GetAllDapperAsync(int limit = 10);
        Task<IEnumerable<Shipment>> GetAllDapperAsync(ShipmentQueryFilter filters);
        Task<Shipment> GetByIdDapperAsync(int id);
        Task<IEnumerable<ShipmentCustomerRouteResponse>> GetShipmentCustomerRouteAsync();

        //Task<Shipment> GetByIdAsync(int id);
        //Task InsertAsync(Shipment shipment);
        //Task UpdateAsync(Shipment shipment);
        //Task DeleteAsync(Shipment shipment);
    }
}

