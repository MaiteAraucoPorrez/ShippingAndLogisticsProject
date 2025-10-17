using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;

namespace ShippingAndLogisticsManagement.Core.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IRouteRepository _routeRepository;
        public ShipmentService(IShipmentRepository shipmentRepository,
            ICustomerRepository customerRepository, 
            IRouteRepository routeRepository)
        {
            _shipmentRepository = shipmentRepository;
            _customerRepository = customerRepository;
            _routeRepository = routeRepository;
        }

        public async Task<IEnumerable<Shipment>> GetAllAsync()
        {
            return await _shipmentRepository.GetAllAsync();
        }

        public async Task<Shipment> GetByIdAsync(int id)
        {
            return await _shipmentRepository.GetByIdAsync(id);
        }

        public async Task InsertAsync(Shipment shipment)
        {
            var customer = await _customerRepository.GetByIdAsync(shipment.CustomerId);
            if (customer == null)
            {
                throw new Exception("El cliente no existe");
            }

            var route = await _routeRepository.GetByIdAsync(shipment.RouteId);
            if (route == null)
                throw new InvalidOperationException("La ruta asignada no existe");

            if (route.IsActive == false)
                throw new InvalidOperationException("No se pueden registrar envios en una ruta inactiva");

            var existingTracking = await _shipmentRepository
            .GetByTrackingNumberAsync(shipment.TrackingNumber);
            if (existingTracking != null)
                throw new InvalidOperationException("El codigo de seguimiento ya existe. Debe ser unico");

            if (shipment.TotalCost <= 0)
                throw new InvalidOperationException("El costo total debe ser mayor que 0");

            if (string.IsNullOrWhiteSpace(shipment.TrackingNumber))
                throw new InvalidOperationException("El número de seguimiento no puede estar vacío");


            // No mas de 3 envios simultaneos por cliente
            var activeShipments = await _shipmentRepository.GetActiveShipmentsByCustomerAsync(shipment.CustomerId);
            if (activeShipments.Count() >= 3)
                throw new InvalidOperationException("El cliente ya tiene 3 envíos activos. No puede registrar mas");

            // Validación del estado inicial
            if (shipment.State != "Pending")
                throw new InvalidOperationException("El estado inicial del envio debe ser 'Pending'.");

            await _shipmentRepository.InsertAsync(shipment);
        }

        public async Task UpdateAsync(Shipment shipment)
        {
            var existing = await _shipmentRepository.GetByIdAsync(shipment.Id);

            if (existing == null)
                throw new KeyNotFoundException("El envio no existe");

            // No se permite marcar "Entregado" sin estados previos
            if (shipment.State == "Delivered" && existing.State != "In transit")
                throw new InvalidOperationException("El envio no puede pasar a 'Delivered' sin haber estado 'In transit'");

            await _shipmentRepository.UpdateAsync(shipment);
        }

        public async Task DeleteAsync(Shipment shipment)
        {
            if (shipment.State == "Delivered")
                throw new InvalidOperationException("No se puede eliminar un envio entregado. Debe marcarse como archivado");

            await _shipmentRepository.DeleteAsync(shipment);
        }
    }
}