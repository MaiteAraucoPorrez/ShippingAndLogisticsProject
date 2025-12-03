using FluentValidation;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;
using System.Text.RegularExpressions;

namespace ShippingAndLogisticsManagement.Infrastructure.Validators
{
    public class WarehouseDtoValidator : AbstractValidator<WarehouseDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        private static readonly Regex CodeRegex = new(@"^[a-zA-Z0-9\-]+$");
        private static readonly Regex CityNameRegex = new(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s\-]+$");
        private static readonly Regex PhoneRegex = new(@"^[\d\s\-\+\(\)]+$");
        private static readonly Regex NameRegex = new(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$");

        private static readonly string[] ValidDepartments = new[]
        {
            "La Paz", "Cochabamba", "Santa Cruz", "Oruro", "Potosí",
            "Chuquisaca", "Tarija", "Beni", "Pando"
        };

        public WarehouseDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // Name validation
            // Name
            RuleFor(w => w.Name)
                .NotEmpty().WithMessage("El nombre del almacén es requerido")
                .Length(5, 100).WithMessage("El nombre debe tener entre 5 y 100 caracteres")
                .Matches(NameRegex).WithMessage("El nombre contiene caracteres inválidos");

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

            // Address
            RuleFor(w => w.Address)
                .NotEmpty().WithMessage("La dirección es requerida")
                .Length(10, 300).WithMessage("La dirección debe tener entre 10 y 300 caracteres");

            // City
            RuleFor(w => w.City)
                .NotEmpty().WithMessage("La ciudad es requerida")
                .Length(3, 100).WithMessage("La ciudad debe tener entre 3 y 100 caracteres")
                .Matches(CityNameRegex).WithMessage("La ciudad solo puede contener letras, espacios y guiones");

            // Department
            RuleFor(w => w.Department)
                .NotEmpty().WithMessage("El departamento es requerido")
                .Must(dept => ValidDepartments.Contains(dept, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Departamento inválido. Debe ser uno de: {string.Join(", ", ValidDepartments)}");

            // Phone
            RuleFor(w => w.Phone)
                .NotEmpty().WithMessage("El teléfono es requerido")
                .Length(7, 20).WithMessage("El teléfono debe tener entre 7 y 20 caracteres")
                .Matches(PhoneRegex).WithMessage("El teléfono solo puede contener dígitos y (+ - ( ))");

            // Email
            RuleFor(w => w.Email)
                .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres")
                .EmailAddress().When(w => !string.IsNullOrEmpty(w.Email))
                .WithMessage("El formato del email no es válido");

            // MaxCapacityM3
            RuleFor(w => w.MaxCapacityM3)
                .GreaterThan(0).WithMessage("La capacidad máxima debe ser mayor a 0")
                .LessThanOrEqualTo(100000).WithMessage("La capacidad máxima no puede exceder 100,000 m³");

            // CurrentCapacityM3
            RuleFor(w => w.CurrentCapacityM3)
                .GreaterThanOrEqualTo(0).WithMessage("La capacidad actual no puede ser negativa")
                .LessThanOrEqualTo(w => w.MaxCapacityM3)
                .WithMessage("La capacidad actual no puede exceder la capacidad máxima");

            // Type
            RuleFor(w => w.Type)
                .IsInEnum()
                .WithMessage("El tipo de almacén no es válido");

            // OperatingHours
            RuleFor(w => w.OperatingHours)
                .MaximumLength(200).When(w => !string.IsNullOrEmpty(w.OperatingHours))
                .WithMessage("El horario no puede exceder 200 caracteres");


            // ManagerName
            RuleFor(w => w.ManagerName)
                .MaximumLength(100).WithMessage("El nombre del encargado es muy largo")
                .Matches(NameRegex).When(w => !string.IsNullOrEmpty(w.ManagerName))
                .WithMessage("El nombre del encargado contiene caracteres inválidos");

            // ID
            RuleFor(w => w.Id)
                .GreaterThanOrEqualTo(0).WithMessage("El ID no puede ser negativo")
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
    }
}