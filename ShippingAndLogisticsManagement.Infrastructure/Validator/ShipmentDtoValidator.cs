using FluentValidation;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;

namespace ShippingAndLogisticsManagement.Infrastructure.Validators
{
    public class ShipmentDtoValidator : AbstractValidator<ShipmentDto>
    {
        private readonly IShipmentService _shipmentService;
        //private readonly IShipmentRepository _shipmentRepository;
        //private readonly IRouteRepository _routeRepository;
        //private readonly ICustomerRepository _customerRepository;
        public readonly IBaseRepository<Shipment> _shipmentRepository;
        public readonly IBaseRepository<Customer> _customerRepository;
        public readonly IBaseRepository<Route> _routeRepository;
        public ShipmentDtoValidator(
            IShipmentService shipmentService,
            //IRouteRepository routeRepository, 
            //ICustomerRepository customerRepository,
            //IShipmentRepository shipmentRepository
            IBaseRepository<Shipment> shipmentRepository,
            IBaseRepository<Customer> customerRepository,
            IBaseRepository<Route> routeRepository
            )
        {
            _shipmentService = shipmentService;
            _routeRepository = routeRepository;
            _customerRepository = customerRepository;
            _shipmentRepository = shipmentRepository;


            // CustomerId obligatorio y mayor que 0
            RuleFor(x => x.CustomerId)
                .GreaterThan(0)
                .WithMessage("El CustomerId debe ser mayor que 0")
                .MustAsync(async (customerId, ct) =>
                 {
                     var customer = await _customerRepository.GetById(customerId);
                     return customer != null;
                 }).WithMessage("El cliente no existe");

            // RouteId obligatorio y mayor que 0
            RuleFor(x => x.RouteId)
                .GreaterThan(0)
                .WithMessage("El RouteId debe ser mayor que 0")
                .MustAsync(async (routeId, ct) =>
                {
                    var route = await _routeRepository.GetById(routeId);
                    return route != null;
                }).WithMessage("La ruta asignada no existe")
                .MustAsync(async (routeId, ct) =>
                {
                    var route = await _routeRepository.GetById(routeId);
                    return route != null && route.IsActive;
                }).WithMessage("No se pueden registrar envíos en una ruta inactiva");

            // TotalCost obligatorio y mayor o igual que 0
            RuleFor(x => x.TotalCost)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El costo total debe ser mayor que 0");

            // State requerido y con valores validos
            RuleFor(x => x.State)
                .NotEmpty().WithMessage("El estado es requerido")
                .Must(BeValidState).WithMessage("El estado debe ser Pending, In transit o Delivered")
                .MaximumLength(20).WithMessage("El estado no puede superar los 20 caracteres")
                .Must((dto, state) =>
                {
                    // Si es creación (Id==0) exigir Pending
                    if (dto.Id == 0)
                        return state == "Pending";
                    return true;
                }).WithMessage("El estado inicial del envío debe ser 'Pending'");

            // Fecha obligatoria y no puede ser menor que hoy
            RuleFor(x => x.ShippingDate)
                .NotEmpty().WithMessage("La fecha de envio es requerida")
                .Must(BeValidShippingDate)
                .WithMessage("La fecha de envio no puede ser anterior a hoy");

            // TrackingNumber obligatorio, formato y longitud
            RuleFor(x => x.TrackingNumber)
                .NotEmpty().WithMessage("El numero de seguimiento es requerido")
                .MaximumLength(20)
                 .WithMessage("El numero de seguimiento no puede superar los 20 caracteres")
                 .MustAsync(async (dto, tracking, ct) =>
                 {
                     var allShipments = await _shipmentRepository.GetAll();
                     var existing = allShipments.FirstOrDefault(s => s.TrackingNumber == tracking);

                     if (existing == null) return true;

                     // Si es update y el tracking pertenece al mismo Id, permitir
                     return dto.Id != 0 && existing.Id == dto.Id;
                 }).WithMessage("El codigo de seguimiento ya existe. Debe ser unico");

        }

        private bool BeValidState(string state)
        {
            var validStates = new[] { "Pending", "In transit", "Delivered" };
            return validStates.Contains(state);
        }

        private bool BeValidShippingDate(string shippingDate)
        {
            if (DateTime.TryParse(shippingDate, out var date))
            {
                return date.Date >= DateTime.Today;
            }
            return false;
        }
    }
}
