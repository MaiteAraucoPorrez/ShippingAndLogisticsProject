using FluentValidation;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;

namespace ShippingAndLogisticsManagement.Infrastructure.Validators
{
    public class RouteDtoValidator : AbstractValidator<RouteDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RouteDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // Origin validation
            RuleFor(r => r.Origin)
                .NotEmpty()
                .WithMessage("El origen es requerido")
                .MinimumLength(3)
                .WithMessage("El origen debe tener al menos 3 caracteres")
                .MaximumLength(100)
                .WithMessage("El origen no puede exceder 100 caracteres")
                .Must(BeAValidCityName)
                .WithMessage("El origen solo puede contener letras, espacios y caracteres acentuados");

            // Destination validation
            RuleFor(r => r.Destination)
                .NotEmpty()
                .WithMessage("El destino es requerido")
                .MinimumLength(3)
                .WithMessage("El destino debe tener al menos 3 caracteres")
                .MaximumLength(100)
                .WithMessage("El destino no puede exceder 100 caracteres")
                .Must(BeAValidCityName)
                .WithMessage("El destino solo puede contener letras, espacios y caracteres acentuados");

            // Origin != Destination
            RuleFor(r => r)
                .Must(route => !route.Origin.Equals(route.Destination, StringComparison.OrdinalIgnoreCase))
                .WithMessage("El origen y destino deben ser diferentes");

            // Distance validation
            RuleFor(r => r.DistanceKm)
                .NotEmpty()
                .WithMessage("La distancia es requerida")
                .GreaterThan(0)
                .WithMessage("La distancia debe ser mayor a 0 km")
                .LessThanOrEqualTo(50000)
                .WithMessage("La distancia no puede exceder 50,000 km")
                .Must(BeValidDistance)
                .WithMessage("La distancia debe tener máximo 2 decimales");

            // Base cost validation
            RuleFor(r => r.BaseCost)
                .NotEmpty()
                .WithMessage("El costo base es requerido")
                .GreaterThanOrEqualTo(0)
                .WithMessage("El costo base no puede ser negativo")
                .LessThanOrEqualTo(1000000)
                .WithMessage("El costo base no puede exceder 1,000,000")
                .Must(BeValidCost)
                .WithMessage("El costo base debe tener máximo 2 decimales");

            // Check for duplicate route (same origin-destination)
            RuleFor(r => r)
                .MustAsync(async (dto, ct) =>
                {
                    var existing = await _unitOfWork.RouteRepository
                        .GetByOriginDestinationAsync(dto.Origin, dto.Destination);

                    // If creating (Id==0), route must not exist
                    // If updating, route can exist only if it's the same route
                    return existing == null || (dto.Id != 0 && existing.Id == dto.Id);
                })
                .WithMessage(dto => $"Ya existe una ruta de {dto.Origin} a {dto.Destination}");

            // ID validation (for updates)
            RuleFor(r => r.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El ID no puede ser negativo")
                .MustAsync(async (dto, id, ct) =>
                {
                    // If updating (Id > 0), route must exist
                    if (id > 0)
                    {
                        var existing = await _unitOfWork.RouteRepository.GetById(id);
                        return existing != null;
                    }
                    return true;
                })
                .WithMessage("La ruta con ese ID no existe")
                .MustAsync(async (dto, id, ct) =>
                {
                    // If updating and changing origin/destination, check for shipments
                    if (id > 0)
                    {
                        var existing = await _unitOfWork.RouteRepository.GetById(id);
                        if (existing != null)
                        {
                            // If origin or destination changed
                            if (!existing.Origin.Equals(dto.Origin, StringComparison.OrdinalIgnoreCase) ||
                                !existing.Destination.Equals(dto.Destination, StringComparison.OrdinalIgnoreCase))
                            {
                                // Check if has shipments
                                var hasShipments = await _unitOfWork.RouteRepository.HasShipmentsAsync(id);
                                return !hasShipments;
                            }
                        }
                    }
                    return true;
                })
                .WithMessage("No se puede modificar origen/destino de una ruta con envíos asociados");

            // Business rule: Cost per km should be reasonable
            RuleFor(r => r)
                .Must(route => ValidateCostPerKm(route.DistanceKm, route.BaseCost))
                .WithMessage("El costo por kilómetro parece inusual. Verifique los datos (debe estar entre 0.1 y 100 por km)");
        }

        /// <summary>
        /// Valida que el nombre de ciudad solo contenga letras, espacios y caracteres acentuados
        /// </summary>
        private bool BeAValidCityName(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName)) return false;

            // Allow letters, spaces, accented characters, hyphens
            return System.Text.RegularExpressions.Regex.IsMatch(
                cityName,
                @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s\-]+$"
            );
        }

        /// <summary>
        /// Valida que la distancia tenga máximo 2 decimales
        /// </summary>
        private bool BeValidDistance(double distance)
        {
            var decimalPlaces = BitConverter.GetBytes(decimal.GetBits((decimal)distance)[3])[2];
            return decimalPlaces <= 2;
        }

        /// <summary>
        /// Valida que el costo tenga máximo 2 decimales
        /// </summary>
        private bool BeValidCost(double cost)
        {
            var decimalPlaces = BitConverter.GetBytes(decimal.GetBits((decimal)cost)[3])[2];
            return decimalPlaces <= 2;
        }

        /// <summary>
        /// Validación de lógica de negocio: costo por km razonable
        /// Alerta si el costo por km es menor a 0.1 o mayor a 100
        /// </summary>
        private bool ValidateCostPerKm(double distance, double cost)
        {
            if (distance <= 0) return true; // Let other validation handle this

            var costPerKm = cost / distance;

            // Alert if cost per km is too low or too high
            return costPerKm >= 0.1 && costPerKm <= 100;
        }
    }
}
