using FluentValidation;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;
using System.Text.RegularExpressions;

namespace ShippingAndLogisticsManagement.Infrastructure.Validators
{
    public class AddressDtoValidator : AbstractValidator<AddressDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        private static readonly string[] ValidDepartments = new[]
        {
            "La Paz", "Cochabamba", "Santa Cruz", "Oruro", "Potosí",
            "Chuquisaca", "Tarija", "Beni", "Pando"
        };

        public AddressDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // CustomerId validation
            RuleFor(a => a.CustomerId)
                .NotEmpty()
                .WithMessage("El ID del cliente es requerido")
                .GreaterThan(0)
                .WithMessage("El ID del cliente debe ser mayor a 0")
                .MustAsync(async (customerId, ct) =>
                {
                    var customer = await _unitOfWork.CustomerRepository.GetById(customerId);
                    return customer != null;
                })
                .WithMessage("El cliente asociado no existe");

            // Street validation
            RuleFor(a => a.Street)
                .NotEmpty()
                .WithMessage("La dirección (calle) es requerida")
                .MinimumLength(5)
                .WithMessage("La dirección debe tener al menos 5 caracteres")
                .MaximumLength(200)
                .WithMessage("La dirección no puede exceder 200 caracteres")
                .Must(BeAValidStreet)
                .WithMessage("La dirección contiene caracteres no permitidos");

            // City validation
            RuleFor(a => a.City)
                .NotEmpty()
                .WithMessage("La ciudad es requerida")
                .MinimumLength(3)
                .WithMessage("La ciudad debe tener al menos 3 caracteres")
                .MaximumLength(100)
                .WithMessage("La ciudad no puede exceder 100 caracteres")
                .Must(BeAValidCityName)
                .WithMessage("La ciudad solo puede contener letras, espacios y guiones");

            // Department validation
            RuleFor(a => a.Department)
                .NotEmpty()
                .WithMessage("El departamento es requerido")
                .Must(BeAValidDepartment)
                .WithMessage(a => $"El departamento '{a.Department}' no es válido. " +
                    $"Debe ser uno de: {string.Join(", ", ValidDepartments)}");

            // Zone validation (optional)
            RuleFor(a => a.Zone)
                .MaximumLength(100)
                .When(a => !string.IsNullOrWhiteSpace(a.Zone))
                .WithMessage("La zona no puede exceder 100 caracteres");

            // PostalCode validation (optional)
            RuleFor(a => a.PostalCode)
                .MaximumLength(20)
                .When(a => !string.IsNullOrWhiteSpace(a.PostalCode))
                .WithMessage("El código postal no puede exceder 20 caracteres");

            // Type validation
            RuleFor(a => a.Type)
                .NotEmpty()
                .WithMessage("El tipo de dirección es requerido")
                .Must(BeAValidAddressType)
                .WithMessage("El tipo debe ser 'Pickup' o 'Delivery'");

            // Reference validation (optional)
            RuleFor(a => a.Reference)
                .MaximumLength(500)
                .When(a => !string.IsNullOrWhiteSpace(a.Reference))
                .WithMessage("La referencia no puede exceder 500 caracteres");

            // Alias validation (optional)
            RuleFor(a => a.Alias)
                .MaximumLength(50)
                .When(a => !string.IsNullOrWhiteSpace(a.Alias))
                .WithMessage("El alias no puede exceder 50 caracteres");

            // ContactName validation (optional)
            RuleFor(a => a.ContactName)
                .MaximumLength(100)
                .When(a => !string.IsNullOrWhiteSpace(a.ContactName))
                .WithMessage("El nombre de contacto no puede exceder 100 caracteres")
                .Must(BeAValidName)
                .When(a => !string.IsNullOrWhiteSpace(a.ContactName))
                .WithMessage("El nombre de contacto solo puede contener letras y espacios");

            // ContactPhone validation (optional)
            RuleFor(a => a.ContactPhone)
                .MinimumLength(7)
                .When(a => !string.IsNullOrWhiteSpace(a.ContactPhone))
                .WithMessage("El teléfono de contacto debe tener al menos 7 caracteres")
                .MaximumLength(20)
                .When(a => !string.IsNullOrWhiteSpace(a.ContactPhone))
                .WithMessage("El teléfono de contacto no puede exceder 20 caracteres")
                .Must(BeAValidPhone)
                .When(a => !string.IsNullOrWhiteSpace(a.ContactPhone))
                .WithMessage("El teléfono de contacto solo puede contener dígitos y caracteres: + - ( )");

            // Latitude validation (optional)
            RuleFor(a => a.Latitude)
                .InclusiveBetween(-90, 90)
                .When(a => a.Latitude.HasValue)
                .WithMessage("La latitud debe estar entre -90 y 90");

            // Longitude validation (optional)
            RuleFor(a => a.Longitude)
                .InclusiveBetween(-180, 180)
                .When(a => a.Longitude.HasValue)
                .WithMessage("La longitud debe estar entre -180 y 180");

            // Business rule: Si tiene latitud, debe tener longitud y viceversa
            RuleFor(a => a)
                .Must(a => (a.Latitude.HasValue && a.Longitude.HasValue) ||
                          (!a.Latitude.HasValue && !a.Longitude.HasValue))
                .WithMessage("Debe proporcionar ambas coordenadas (latitud y longitud) o ninguna");

            // Business rule: Límite de direcciones por cliente
            RuleFor(a => a.CustomerId)
                .MustAsync(async (dto, customerId, ct) =>
                {
                    // Solo validar en creación (Id == 0)
                    if (dto.Id == 0)
                    {
                        var count = await _unitOfWork.AddressRepository.CountActiveAddressesAsync(customerId);
                        return count < 10;
                    }
                    return true;
                })
                .WithMessage("El cliente ha alcanzado el límite máximo de 10 direcciones activas");

            // ID validation (for updates)
            RuleFor(a => a.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El ID no puede ser negativo")
                .MustAsync(async (dto, id, ct) =>
                {
                    // Si es actualización (Id > 0), verificar que existe
                    if (id > 0)
                    {
                        var existing = await _unitOfWork.AddressRepository.GetById(id);
                        return existing != null;
                    }
                    return true;
                })
                .WithMessage("La dirección con ese ID no existe");
        }

        private bool BeAValidStreet(string street)
        {
            if (string.IsNullOrWhiteSpace(street)) return false;
            // Permitir letras, números, espacios, puntos, comas, guiones, #
            return Regex.IsMatch(street, @"^[a-zA-Z0-9\s\.,\-#áéíóúÁÉÍÓÚñÑüÜ]+$");
        }

        private bool BeAValidCityName(string city)
        {
            if (string.IsNullOrWhiteSpace(city)) return false;
            // Permitir letras, espacios, guiones
            return Regex.IsMatch(city, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s\-]+$");
        }

        private bool BeAValidDepartment(string department)
        {
            if (string.IsNullOrWhiteSpace(department)) return false;
            return ValidDepartments.Contains(department, StringComparer.OrdinalIgnoreCase);
        }

        private bool BeAValidAddressType(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) return false;
            return type.Equals("Pickup", StringComparison.OrdinalIgnoreCase) ||
                   type.Equals("Delivery", StringComparison.OrdinalIgnoreCase);
        }

        private bool BeAValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            return Regex.IsMatch(name, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$");
        }

        private bool BeAValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return Regex.IsMatch(phone, @"^[\d\s\-\+\(\)]+$");
        }
    }
}