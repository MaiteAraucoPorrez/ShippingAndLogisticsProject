using FluentValidation;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;
using System.Text.RegularExpressions;

namespace ShippingAndLogisticsManagement.Infrastructure.Validators
{
    public class DriverDtoValidator : AbstractValidator<DriverDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        private static readonly Regex NameRegex = new(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$");
        private static readonly Regex PhoneRegex = new(@"^[\d\s\-\+\(\)]+$");
        public DriverDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // FullName
            RuleFor(d => d.FullName)
                .NotEmpty().WithMessage("El nombre completo es requerido")
                .MinimumLength(5).WithMessage("El nombre debe tener al menos 5 caracteres")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres")
                .Must(BeAValidName).WithMessage("El nombre solo puede contener letras y espacios");

            // LicenseNumber
            RuleFor(d => d.LicenseNumber)
                .NotEmpty().WithMessage("El número de licencia es requerido")
                .MaximumLength(50).WithMessage("La licencia no puede exceder 50 caracteres")
                .MustAsync(async (dto, license, ct) =>
                {
                    var existing = await _unitOfWork.DriverRepository.GetByLicenseNumberAsync(license);
                    return existing == null || (dto.Id != 0 && existing.Id == dto.Id);
                }).WithMessage("Ya existe un conductor con ese número de licencia");

            // Phone
            RuleFor(d => d.Phone)
                .NotEmpty().WithMessage("El teléfono es requerido")
                .MinimumLength(7).WithMessage("El teléfono debe tener al menos 7 caracteres")
                .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
                .Must(BeAValidPhone).WithMessage("El teléfono solo puede contener dígitos y caracteres: + - ( )");

            // Email
            RuleFor(d => d.Email)
                .NotEmpty().WithMessage("El email es requerido")
                .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres")
                .EmailAddress().WithMessage("El email no tiene un formato válido");

            // Address
            RuleFor(d => d.Address)
                .MaximumLength(300).When(d => !string.IsNullOrWhiteSpace(d.Address))
                .WithMessage("La dirección no puede exceder 300 caracteres");

            // City
            RuleFor(d => d.City)
                .MaximumLength(100).When(d => !string.IsNullOrWhiteSpace(d.City))
                .WithMessage("La ciudad no puede exceder 100 caracteres");

            // DateOfBirth
            RuleFor(d => d.DateOfBirth)
                .NotEmpty().WithMessage("La fecha de nacimiento es requerida")
                .LessThan(DateTime.Now).WithMessage("La fecha de nacimiento debe ser pasada")
                .Must(BeAtLeast18YearsOld).WithMessage("El conductor debe ser mayor de 18 años");

            // Status
            RuleFor(d => d.Status)
                .IsInEnum()
                .WithMessage("El estado seleccionado no es válido");

            // TotalDeliveries
            RuleFor(d => d.TotalDeliveries)
                .GreaterThanOrEqualTo(0).WithMessage("Las entregas no pueden ser negativas");

            // CurrentVehicleId
            RuleFor(d => d.CurrentVehicleId)
                .MustAsync(async (vehicleId, ct) =>
                {
                    if (!vehicleId.HasValue) return true;
                    var vehicle = await _unitOfWork.VehicleRepository.GetById(vehicleId.Value);
                    return vehicle != null;
                }).When(d => d.CurrentVehicleId.HasValue)
                .WithMessage("El vehículo asignado no existe");
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

        private bool BeAtLeast18YearsOld(DateTime dateOfBirth)
        {
            var age = DateTime.Now.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Now.AddYears(-age)) age--;
            return age >= 18;
        }
    }
}