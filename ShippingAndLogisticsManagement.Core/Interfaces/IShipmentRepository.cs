using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    public interface IShipmentRepository
    {
        Task<IEnumerable<Shipment>> GetAllAsync();
        Task<Shipment> GetByIdAsync(int id);
        Task InsertAsync(Shipment shipment);
        Task UpdateAsync(Shipment shipment);
        Task DeleteAsync(Shipment shipment);
        Task<IEnumerable<Shipment>> GetActiveShipmentsByCustomerAsync(int customerId);
        Task<Shipment> GetByTrackingNumberAsync(string trackingNumber);
    }
}

