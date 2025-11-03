using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Exceptions;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.QueryFilters;
using System.Net;

namespace ShippingAndLogisticsManagement.Core.Services
{
    public class ShipmentService : IShipmentService
    {
        //public readonly IShipmentRepository _shipmentRepository;
        //public readonly ICustomerRepository _customerRepository;
        //public readonly IRouteRepository _routeRepository;
        //public readonly IBaseRepository<Shipment> _shipmentRepository;
        //public readonly IBaseRepository<Customer> _customerRepository;
        //public readonly IBaseRepository<Route> _routeRepository;
        public readonly IUnitOfWork _unitOfWork;
        public ShipmentService(
            //IShipmentRepository shipmentRepository,
            //ICustomerRepository customerRepository, 
            //IRouteRepository routeRepository,
            //IBaseRepository<Shipment> shipmentRepository,
            //IBaseRepository<Customer> customerRepository,
            //IBaseRepository<Route> routeRepository
            IUnitOfWork unitOfWork
            )
        {
            //_shipmentRepository = shipmentRepository;
            //_customerRepository = customerRepository;
            //_routeRepository = routeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseData> GetAllAsync(ShipmentQueryFilter shipmentQueryFilter)
        {
            // Validar que el filtro no sea null
            if (shipmentQueryFilter == null)
                throw new ArgumentNullException(nameof(shipmentQueryFilter), "El filtro de búsqueda no puede ser nulo");

            // Validar CustomerId si es proporcionado
            if (shipmentQueryFilter.CustomerId.HasValue && shipmentQueryFilter.CustomerId <= 0)
                throw new ArgumentException("CustomerId debe ser mayor a 0", nameof(shipmentQueryFilter.CustomerId));

            // Validar RouteId si es proporcionado
            if (shipmentQueryFilter.RouteId.HasValue && shipmentQueryFilter.RouteId <= 0)
                throw new ArgumentException("RouteId debe ser mayor a 0", nameof(shipmentQueryFilter.RouteId));

            // Validar TotalCost
            if (shipmentQueryFilter.TotalCost < 0)
                throw new ArgumentException("TotalCost no puede ser negativo", nameof(shipmentQueryFilter.TotalCost));

            // Validar State (es required)
            if (string.IsNullOrWhiteSpace(shipmentQueryFilter.State))
                throw new ArgumentException("State no puede estar vacío", nameof(shipmentQueryFilter.State));

            // Validar TrackingNumber si es proporcionado
            if (!string.IsNullOrWhiteSpace(shipmentQueryFilter.TrackingNumber) &&
                shipmentQueryFilter.TrackingNumber.Length < 3)
                throw new ArgumentException("TrackingNumber debe tener al menos 3 caracteres",
                    nameof(shipmentQueryFilter.TrackingNumber));

            // Validar ShippingDate si es proporcionado (no puede ser futura)
            if (shipmentQueryFilter.ShippingDate.HasValue &&
                shipmentQueryFilter.ShippingDate > DateTime.Now)
                throw new ArgumentException("ShippingDate no puede ser una fecha futura",
                    nameof(shipmentQueryFilter.ShippingDate));

            var shipments = await _unitOfWork.ShipmentRepository.GetAll();

            // Aplicar filtros
            if (shipmentQueryFilter.CustomerId.HasValue)
            {
                shipments = shipments.Where(p => p.CustomerId == shipmentQueryFilter.CustomerId);
            }

            if (shipmentQueryFilter.ShippingDate.HasValue)
            {
                shipments = shipments.Where(p => p.ShippingDate.ToShortDateString() ==
                    shipmentQueryFilter.ShippingDate?.ToShortDateString());
            }

            if (!string.IsNullOrWhiteSpace(shipmentQueryFilter.State))
            {
                shipments = shipments.Where(p => p.State == shipmentQueryFilter.State);
            }

            if (shipmentQueryFilter.RouteId.HasValue)
            {
                shipments = shipments.Where(p => p.RouteId == shipmentQueryFilter.RouteId);
            }

            if (shipmentQueryFilter.TotalCost > 0)
            {
                shipments = shipments.Where(p => p.TotalCost == shipmentQueryFilter.TotalCost);
            }

            if (!string.IsNullOrWhiteSpace(shipmentQueryFilter.TrackingNumber))
            {
                shipments = shipments.Where(p => p.TrackingNumber == shipmentQueryFilter.TrackingNumber);
            }

            //return await _shipmentRepository.GetAll();
            //return await _unitOfWork.ShipmentRepository.GetAll();
            //return shipments;

            if (shipmentQueryFilter.CustomerId != null)
            {
                shipments = shipments.Where(x => x.CustomerId == shipmentQueryFilter.CustomerId);
            }
            if (shipmentQueryFilter.ShippingDate != null)
            {
                shipments = shipments.Where(
                    x => x.ShippingDate.ToShortDateString() == shipmentQueryFilter.ShippingDate?.ToShortDateString());
            }
            if (shipmentQueryFilter.State != null)
            {
                shipments = shipments.Where(x => x.State.ToLower().Contains(shipmentQueryFilter.State.ToLower()));
            }

            var pagedPosts = PagedList<object>.Create(
                shipments, shipmentQueryFilter.PageNumber, shipmentQueryFilter.PageSize);
            if (pagedPosts.Any())
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Registros de envios recuperados correctamente" } },
                    Pagination = pagedPosts,
                    StatusCode = HttpStatusCode.OK
                };
            }
            else
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = "No fue posible recuperar la cantidad de registros" } },
                    Pagination = pagedPosts,
                    StatusCode = HttpStatusCode.OK
                };
            }
        }

        public async Task<IEnumerable<Shipment>> GetAllDapperAsync()
        {
            var shipments = await _unitOfWork.ShipmentRepository.GetAllDapperAsync(5);

            return shipments;
        }

        public async Task<IEnumerable<ShipmentCustomerRouteResponse>> GetShipmentCustomerRouteAsync()
        {
            var shipments = await _unitOfWork.ShipmentRepository.GetShipmentCustomerRouteAsync();
            return shipments;
        }

        public async Task<Shipment> GetByIdAsync(int id)
        {
            //return await _shipmentRepository.GetById(id);
            return await _unitOfWork.ShipmentRepository.GetById(id);
        }

        public async Task InsertAsync(Shipment shipment)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(shipment.CustomerId);
            if (customer == null)
            {
                throw new BusinessException("El cliente no existe");
            }

            var route = await _unitOfWork.RouteRepository.GetById(shipment.RouteId);
            if (route == null)
                throw new BusinessException("La ruta asignada no existe");

            if (route.IsActive == false)
                throw new BusinessException("No se pueden registrar envios en una ruta inactiva");

            var allShipments = await _unitOfWork.ShipmentRepository.GetAll();
            var existingTracking = allShipments.FirstOrDefault(s => s.TrackingNumber == shipment.TrackingNumber);
            if (existingTracking != null)
                throw new BusinessException("El codigo de seguimiento ya existe. Debe ser unico");


            if (shipment.TotalCost <= 0)
                throw new BusinessException("El costo total debe ser mayor que 0");

            if (string.IsNullOrWhiteSpace(shipment.TrackingNumber))
                throw new BusinessException("El número de seguimiento no puede estar vacío");


            // No mas de 3 envios simultaneos por cliente con LINQ
            var activeShipments = allShipments
                .Where(s => s.CustomerId == shipment.CustomerId && s.State != "Delivered")
                .ToList();

            if (activeShipments.Count >= 3)
                throw new BusinessException("El cliente ya tiene 3 envíos activos. No puede registrar mas");


            // Validación del estado inicial
            if (shipment.State != "Pending")
                throw new BusinessException("El estado inicial del envio debe ser 'Pending'.");

            //await _shipmentRepository.Add(shipment);
            await _unitOfWork.ShipmentRepository.Add(shipment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Shipment shipment)
        {
            var existing = await _unitOfWork.ShipmentRepository.GetById(shipment.Id);

            if (existing == null)
                throw new KeyNotFoundException("El envio no existe");

            // No se permite marcar "Entregado" sin estados previos
            if (shipment.State == "Delivered" && existing.State != "In transit")
                throw new InvalidOperationException("El envio no puede pasar a 'Delivered' sin haber estado 'In transit'");

            //await _shipmentRepository.Update(shipment);
            await _unitOfWork.ShipmentRepository.Update(shipment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            //if (shipment.State == "Delivered")
            //    throw new InvalidOperationException("No se puede eliminar un envio entregado. Debe marcarse como archivado");
            //await _shipmentRepository.Delete(id);
            await _unitOfWork.ShipmentRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}