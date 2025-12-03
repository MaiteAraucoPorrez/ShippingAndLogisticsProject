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

        // Optimización: Regex compilados estáticos para mejor rendimiento
        private static readonly Regex StreetRegex = new(@"^[a-zA-Z0-9\s\.,\-#áéíóúÁÉÍÓÚñÑüÜ]+$");
        private static readonly Regex CityNameRegex = new(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s\-]+$");
        private static readonly Regex NameRegex = new(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$");
        private static readonly Regex PhoneRegex = new(@"^[\d\s\-\+\(\)]+$");

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
                .NotEmpty().WithMessage("El ID del cliente es requerido")
                .GreaterThan(0).WithMessage("El ID del cliente debe ser mayor a 0")
                .MustAsync(async (customerId, ct) =>
                {
                    // Validamos que el cliente exista en la BD
                    var customer = await _unitOfWork.CustomerRepository.GetById(customerId);
                    return customer != null;
                })
                .WithMessage("El cliente asociado no existe");

            // Usamos la sintaxis (dto, customerId, context) para acceder al ID del DTO
            RuleFor(a => a.CustomerId)
                .MustAsync(async (dto, customerId, ct) =>
                {
                    // Solo validar límite si es una creación (Id == 0)
                    if (dto.Id == 0)
                    {
                        var count = await _unitOfWork.AddressRepository.CountActiveAddressesAsync(customerId);
                        return count < 10;
                    }
                    return true;
                })
                .WithMessage("El cliente ha alcanzado el límite máximo de 10 direcciones activas");

            // Street validation
            RuleFor(a => a.Street)
                .NotEmpty().WithMessage("La dirección es requerida")
                .Length(5, 200).WithMessage("La dirección debe tener entre 5 y 200 caracteres")
                .Matches(StreetRegex).WithMessage("La dirección contiene caracteres no permitidos");

            // City validation
            RuleFor(a => a.City)
                .NotEmpty().WithMessage("La ciudad es requerida")
                .Length(3, 100).WithMessage("La ciudad debe tener entre 3 y 100 caracteres")
                .Matches(CityNameRegex).WithMessage("La ciudad solo puede contener letras, espacios y guiones");

            // Department validation
            RuleFor(a => a.Department)
                .NotEmpty().WithMessage("El departamento es requerido")
                .Must(dept => ValidDepartments.Contains(dept, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Departamento inválido. Debe ser uno de: {string.Join(", ", ValidDepartments)}");

            // Zone validation
            RuleFor(a => a.Zone)
                .MaximumLength(100).WithMessage("La zona no puede exceder 100 caracteres");

            // Usamos IsInEnum() que verifica que el valor int corresponda a un valor definido en el Enum
            RuleFor(a => a.Type)
                .IsInEnum()
                .WithMessage("El tipo de dirección no es válido (Debe ser Pickup o Delivery)");

            // Reference validation
            RuleFor(a => a.Reference)
                .MaximumLength(500).WithMessage("La referencia no puede exceder 500 caracteres");

            // ContactName validation
            RuleFor(a => a.ContactName)
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres")
                .Matches(NameRegex).When(a => !string.IsNullOrEmpty(a.ContactName))
                .WithMessage("El nombre solo puede contener letras y espacios");

            // ContactPhone validation
            RuleFor(a => a.ContactPhone)
                .Length(7, 20).When(a => !string.IsNullOrEmpty(a.ContactPhone))
                .WithMessage("El teléfono debe tener entre 7 y 20 caracteres")
                .Matches(PhoneRegex).When(a => !string.IsNullOrEmpty(a.ContactPhone))
                .WithMessage("El teléfono solo puede contener números y (+ - ( ))");

            // ID validation (Update scenario)
            RuleFor(a => a.Id)
                .GreaterThanOrEqualTo(0).WithMessage("El ID no puede ser negativo")
                .MustAsync(async (id, ct) =>
                {
                    if (id > 0)
                    {
                        var existing = await _unitOfWork.AddressRepository.GetById(id);
                        return existing != null;
                    }
                    return true;
                })
                .WithMessage("La dirección con ese ID no existe para ser actualizada");
        }
    }
}