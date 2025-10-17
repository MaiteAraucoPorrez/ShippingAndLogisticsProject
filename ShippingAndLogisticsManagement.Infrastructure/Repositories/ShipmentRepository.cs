using Microsoft.EntityFrameworkCore;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.Data;

namespace ShippingAndLogisticsManagement.Infrastructure.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly LogisticContext _context;

        public ShipmentRepository(LogisticContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Shipment>> GetAllAsync()
        {
            var shipments = await _context.Shipments.ToListAsync();
            return shipments;
        }

        public async Task<Shipment> GetByIdAsync(int id)
        {
            var shipment = await _context.Shipments.FirstOrDefaultAsync(x => x.Id == id);
            return shipment;
        }

        public async Task InsertAsync(Shipment shipment)
        {
            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Shipment shipment)
        {
            _context.Shipments.Update(shipment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Shipment shipment)
        {
            _context.Shipments.Remove(shipment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Shipment>> GetActiveShipmentsByCustomerAsync(int customerId)
        {
            return await _context.Shipments
                .Where(s => s.CustomerId == customerId && s.State != "Delivered")
                .ToListAsync();
        }

        public async Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber)
        {
            return await _context.Shipments
                .FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber);
        }

    }
}