using FluentValidation;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;
using System.Text.RegularExpressions;

namespace ShippingAndLogisticsManagement.Infrastructure.Validators
{
    public class DriverDtoValidator : AbstractValidator<DriverDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DriverDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // FullName
            RuleFor(d => d.FullName)
                .NotEmpty().WithMessage("El nombre completo es requerido")
                .MinimumLength(5).WithMessage("El nombre debe tener al menos 5 caracteres")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres")
                .Must(BeAValidName).WithMessage("El nombre solo puede contener letras y espacios");

            // IdentityDocument
            RuleFor(d => d.IdentityDocument)
                .NotEmpty().WithMessage("El documento de identidad es requerido")
                .MaximumLength(50).WithMessage("El documento no puede exceder 50 caracteres")
                .MustAsync(async (dto, doc, ct) =>
                {
                    var existing = await _unitOfWork.DriverRepository.GetByIdentityDocumentAsync(doc);
                    return existing == null || (dto.Id != 0 && existing.Id == dto.Id);
                }).WithMessage("Ya existe un conductor con ese documento de identidad");

            // LicenseNumber
            RuleFor(d => d.LicenseNumber)
                .NotEmpty().WithMessage("El número de licencia es requerido")
                .MaximumLength(50).WithMessage("La licencia no puede exceder 50 caracteres")
                .MustAsync(async (dto, license, ct) =>
                {
                    var existing = await _unitOfWork.DriverRepository.GetByLicenseNumberAsync(license);
                    return existing == null || (dto.Id != 0 && existing.Id == dto.Id);
                }).WithMessage("Ya existe un conductor con ese número de licencia");

            // LicenseCategory
            RuleFor(d => d.LicenseCategory)
                .NotEmpty().WithMessage("La categoría de licencia es requerida")
                .MaximumLength(20).WithMessage("La categoría no puede exceder 20 caracteres");

            // LicenseIssueDate
            RuleFor(d => d.LicenseIssueDate)
                .NotEmpty().WithMessage("La fecha de emisión de licencia es requerida")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de emisión no puede ser futura");

            // LicenseExpiryDate
            RuleFor(d => d.LicenseExpiryDate)
                .NotEmpty().WithMessage("La fecha de vencimiento es requerida")
                .GreaterThan(DateTime.Now).WithMessage("La licencia está vencida")
                .GreaterThan(d => d.LicenseIssueDate).WithMessage("El vencimiento debe ser posterior a la emisión");

            // Phone
            RuleFor(d => d.Phone)
                .NotEmpty().WithMessage("El teléfono es requerido")
                .MinimumLength(7).WithMessage("El teléfono debe tener al menos 7 caracteres")
                .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
                .Must(BeAValidPhone).WithMessage("El teléfono solo puede contener dígitos y caracteres: + - ( )");

            // AlternativePhone
            RuleFor(d => d.AlternativePhone)
                .MaximumLength(20).When(d => !string.IsNullOrWhiteSpace(d.AlternativePhone))
                .Must(BeAValidPhone).When(d => !string.IsNullOrWhiteSpace(d.AlternativePhone))
                .WithMessage("El teléfono alternativo solo puede contener dígitos y caracteres: + - ( )");

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

            // HireDate
            RuleFor(d => d.HireDate)
                .NotEmpty().WithMessage("La fecha de contratación es requerida")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de contratación no puede ser futura");

            // ContractEndDate
            RuleFor(d => d.ContractEndDate)
                .GreaterThan(d => d.HireDate).When(d => d.ContractEndDate.HasValue)
                .WithMessage("La fecha de fin debe ser posterior a la contratación");

            // YearsOfExperience
            RuleFor(d => d.YearsOfExperience)
                .GreaterThanOrEqualTo(0).WithMessage("Los años de experiencia no pueden ser negativos")
                .LessThanOrEqualTo(60).WithMessage("Los años de experiencia parecen inusuales");

            // Status
            RuleFor(d => d.Status)
                .NotEmpty().WithMessage("El estado es requerido")
                .Must(BeAValidStatus).WithMessage("Estado inválido. Use: Available, OnRoute, OffDuty, OnLeave");

            // AverageRating
            RuleFor(d => d.AverageRating)
                .InclusiveBetween(1.0, 5.0).When(d => d.AverageRating.HasValue)
                .WithMessage("La calificación debe estar entre 1 y 5");

            // TotalDeliveries
            RuleFor(d => d.TotalDeliveries)
                .GreaterThanOrEqualTo(0).WithMessage("Las entregas no pueden ser negativas");

            // EmergencyContactPhone
            RuleFor(d => d.EmergencyContactPhone)
                .Must(BeAValidPhone).When(d => !string.IsNullOrWhiteSpace(d.EmergencyContactPhone))
                .WithMessage("El teléfono de emergencia solo puede contener dígitos y caracteres: + - ( )");

            // BloodType
            RuleFor(d => d.BloodType)
                .Must(BeAValidBloodType).When(d => !string.IsNullOrWhiteSpace(d.BloodType))
                .WithMessage("Tipo de sangre inválido. Use: A+, A-, B+, B-, AB+, AB-, O+, O-");

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

        private bool BeAValidStatus(string status)
        {
            var validStatuses = new[] { "Available", "OnRoute", "OffDuty", "OnLeave" };
            return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
        }

        private bool BeAValidBloodType(string bloodType)
        {
            if (string.IsNullOrWhiteSpace(bloodType)) return true;
            var validTypes = new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
            return validTypes.Contains(bloodType, StringComparer.OrdinalIgnoreCase);
        }
    }
}