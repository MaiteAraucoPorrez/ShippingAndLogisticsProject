using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    public interface IShipmentService
    {
        Task<IEnumerable<Shipment>> GetAllAsync();
        Task<Shipment> GetByIdAsync(int id);
        Task InsertAsync(Shipment shipment);
        Task UpdateAsync(Shipment shipment);
        Task DeleteAsync(Shipment shipment);
    }
}
