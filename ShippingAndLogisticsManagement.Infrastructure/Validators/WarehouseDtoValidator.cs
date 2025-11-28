using FluentValidation;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;
using System.Text.RegularExpressions;

namespace ShippingAndLogisticsManagement.Infrastructure.Validators
{
    public class WarehouseDtoValidator : AbstractValidator<WarehouseDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        private static readonly string[] ValidDepartments = new[]
        {
            "La Paz", "Cochabamba", "Santa Cruz", "Oruro", "Potosí",
            "Chuquisaca", "Tarija", "Beni", "Pando"
        };

        public WarehouseDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // Name validation
            RuleFor(w => w.Name)
                .NotEmpty()
                .WithMessage("El nombre del almacén es requerido")
                .MinimumLength(5)
                .WithMessage("El nombre debe tener al menos 5 caracteres")
                .MaximumLength(100)
                .WithMessage("El nombre no puede exceder 100 caracteres");

            // Code validation
            RuleFor(w => w.Code)
                .NotEmpty()
                .WithMessage("El código del almacén es requerido")
                .MinimumLength(3)
                .WithMessage("El código debe tener al menos 3 caracteres")
                .MaximumLength(20)
                .WithMessage("El código no puede exceder 20 caracteres")
                .Must(BeAValidCode)
                .WithMessage("El código solo puede contener letras, números y guiones")
                .MustAsync(async (dto, code, ct) =>
                {
                    var exists = await _unitOfWork.WarehouseRepository
                        .CodeExistsAsync(code, dto.Id > 0 ? dto.Id : null);
                    return !exists;
                })
                .WithMessage(dto => $"Ya existe un almacén con el código '{dto.Code}'");

            // Address validation
            RuleFor(w => w.Address)
                .NotEmpty()
                .WithMessage("La dirección es requerida")
                .MinimumLength(10)
                .WithMessage("La dirección debe tener al menos 10 caracteres")
                .MaximumLength(300)
                .WithMessage("La dirección no puede exceder 300 caracteres");

            // City validation
            RuleFor(w => w.City)
                .NotEmpty()
                .WithMessage("La ciudad es requerida")
                .MinimumLength(3)
                .WithMessage("La ciudad debe tener al menos 3 caracteres")
                .MaximumLength(100)
                .WithMessage("La ciudad no puede exceder 100 caracteres")
                .Must(BeAValidCityName)
                .WithMessage("La ciudad solo puede contener letras, espacios y guiones");

            // Department validation
            RuleFor(w => w.Department)
                .NotEmpty()
                .WithMessage("El departamento es requerido")
                .Must(BeAValidDepartment)
                .WithMessage(w => $"El departamento '{w.Department}' no es válido. " +
                    $"Debe ser uno de: {string.Join(", ", ValidDepartments)}");

            // Phone validation
            RuleFor(w => w.Phone)
                .NotEmpty()
                .WithMessage("El teléfono es requerido")
                .MinimumLength(7)
                .WithMessage("El teléfono debe tener al menos 7 caracteres")
                .MaximumLength(20)
                .WithMessage("El teléfono no puede exceder 20 caracteres")
                .Must(BeAValidPhone)
                .WithMessage("El teléfono solo puede contener dígitos y caracteres: + - ( )");

            // Email validation (optional)
            RuleFor(w => w.Email)
                .EmailAddress()
                .When(w => !string.IsNullOrWhiteSpace(w.Email))
                .WithMessage("El email no tiene un formato válido")
                .MaximumLength(100)
                .When(w => !string.IsNullOrWhiteSpace(w.Email))
                .WithMessage("El email no puede exceder 100 caracteres");

            // MaxCapacityM3 validation
            RuleFor(w => w.MaxCapacityM3)
                .NotEmpty()
                .WithMessage("La capacidad máxima es requerida")
                .GreaterThan(0)
                .WithMessage("La capacidad máxima debe ser mayor a 0")
                .LessThanOrEqualTo(100000)
                .WithMessage("La capacidad máxima no puede exceder 100,000 m³");

            // CurrentCapacityM3 validation
            RuleFor(w => w.CurrentCapacityM3)
                .GreaterThanOrEqualTo(0)
                .WithMessage("La capacidad actual no puede ser negativa")
                .LessThanOrEqualTo(w => w.MaxCapacityM3)
                .WithMessage("La capacidad actual no puede exceder la capacidad máxima");

            // Type validation
            RuleFor(w => w.Type)
                .NotEmpty()
                .WithMessage("El tipo de almacén es requerido")
                .Must(BeAValidWarehouseType)
                .WithMessage("El tipo debe ser 'Central', 'Regional' o 'Local'");

            // OperatingHours validation (optional)
            RuleFor(w => w.OperatingHours)
                .MaximumLength(200)
                .When(w => !string.IsNullOrWhiteSpace(w.OperatingHours))
                .WithMessage("El horario no puede exceder 200 caracteres");

            // ManagerName validation (optional)
            RuleFor(w => w.ManagerName)
                .MaximumLength(100)
                .When(w => !string.IsNullOrWhiteSpace(w.ManagerName))
                .WithMessage("El nombre del encargado no puede exceder 100 caracteres")
                .Must(BeAValidName)
                .When(w => !string.IsNullOrWhiteSpace(w.ManagerName))
                .WithMessage("El nombre del encargado solo puede contener letras y espacios");

            // Latitude validation (optional)
            RuleFor(w => w.Latitude)
                .InclusiveBetween(-90, 90)
                .When(w => w.Latitude.HasValue)
                .WithMessage("La latitud debe estar entre -90 y 90");

            // Longitude validation (optional)
            RuleFor(w => w.Longitude)
                .InclusiveBetween(-180, 180)
                .When(w => w.Longitude.HasValue)
                .WithMessage("La longitud debe estar entre -180 y 180");

            // OpeningDate validation
            RuleFor(w => w.OpeningDate)
                .NotEmpty()
                .WithMessage("La fecha de apertura es requerida")
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("La fecha de apertura no puede ser futura");

            // Notes validation (optional)
            RuleFor(w => w.Notes)
                .MaximumLength(500)
                .When(w => !string.IsNullOrWhiteSpace(w.Notes))
                .WithMessage("Las notas no pueden exceder 500 caracteres");

            // Business rule: Si tiene latitud, debe tener longitud y viceversa
            RuleFor(w => w)
                .Must(w => (w.Latitude.HasValue && w.Longitude.HasValue) ||
                          (!w.Latitude.HasValue && !w.Longitude.HasValue))
                .WithMessage("Debe proporcionar ambas coordenadas (latitud y longitud) o ninguna");

            // ID validation (for updates)
            RuleFor(w => w.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El ID no puede ser negativo")
                .MustAsync(async (dto, id, ct) =>
                {
                    if (id > 0)
                    {
                        var existing = await _unitOfWork.WarehouseRepository.GetById(id);
                        return existing != null;
                    }
                    return true;
                })
                .WithMessage("El almacén con ese ID no existe");
        }

        private bool BeAValidCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return false;
            return Regex.IsMatch(code, @"^[a-zA-Z0-9\-]+$");
        }

        private bool BeAValidCityName(string city)
        {
            if (string.IsNullOrWhiteSpace(city)) return false;
            return Regex.IsMatch(city, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s\-]+$");
        }

        private bool BeAValidDepartment(string department)
        {
            if (string.IsNullOrWhiteSpace(department)) return false;
            return ValidDepartments.Contains(department, StringComparer.OrdinalIgnoreCase);
        }

        private bool BeAValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return Regex.IsMatch(phone, @"^[\d\s\-\+\(\)]+$");
        }

        private bool BeAValidWarehouseType(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) return false;
            return type.Equals("Central", StringComparison.OrdinalIgnoreCase) ||
                   type.Equals("Regional", StringComparison.OrdinalIgnoreCase) ||
                   type.Equals("Local", StringComparison.OrdinalIgnoreCase);
        }

        private bool BeAValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            return Regex.IsMatch(name, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$");
        }
    }
}