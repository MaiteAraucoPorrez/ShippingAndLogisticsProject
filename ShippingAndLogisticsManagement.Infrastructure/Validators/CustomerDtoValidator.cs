using FluentValidation;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;
using System.Text.RegularExpressions;

namespace ShippingAndLogisticsManagement.Infrastructure.Validators
{
    public class CustomerDtoValidator : AbstractValidator<CustomerDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // Name validation
            RuleFor(c => c.Name)
                .NotEmpty()
                .WithMessage("El nombre es requerido")
                .MinimumLength(3)
                .WithMessage("El nombre debe tener al menos 3 caracteres")
                .MaximumLength(100)
                .WithMessage("El nombre no puede exceder 100 caracteres")
                .Must(BeAValidName)
                .WithMessage("El nombre solo puede contener letras, espacios y caracteres acentuados");

            // Email validation
            RuleFor(c => c.Email)
                .NotEmpty()
                .WithMessage("El email es requerido")
                .MaximumLength(100)
                .WithMessage("El email no puede exceder 100 caracteres")
                .EmailAddress()
                .WithMessage("El email no tiene un formato válido")
                .MustAsync(async (dto, email, ct) =>
                {
                    var existing = await _unitOfWork.CustomerRepository.GetByEmailAsync(email);
                    // If creating (Id==0), email must not exist
                    // If updating, email can exist only if it's the same customer
                    return existing == null || (dto.Id != 0 && existing.Id == dto.Id);
                })
                .WithMessage("Ya existe un cliente con ese email")
                .MustAsync(async (dto, email, ct) =>
                {
                    // Check email domain limit (max 5 per domain)
                    if (dto.Id == 0) // Only on create
                    {
                        var domain = email.Split('@').Last();
                        var count = await _unitOfWork.CustomerRepository.CountByEmailDomainAsync(domain);
                        return count < 5;
                    }
                    return true;
                })
                .WithMessage(dto => $"Se alcanzó el límite de 5 clientes con el dominio @{dto.Email.Split('@').Last()}");

            // Phone validation
            RuleFor(c => c.Phone)
                .NotEmpty()
                .WithMessage("El teléfono es requerido")
                .MinimumLength(7)
                .WithMessage("El teléfono debe tener al menos 7 caracteres")
                .MaximumLength(20)
                .WithMessage("El teléfono no puede exceder 20 caracteres")
                .Must(BeAValidPhone)
                .WithMessage("El teléfono solo puede contener dígitos, espacios y los caracteres: + - ( )");

            // ID validation (for updates)
            RuleFor(c => c.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El ID no puede ser negativo")
                .MustAsync(async (dto, id, ct) =>
                {
                    // If updating (Id > 0), customer must exist
                    if (id > 0)
                    {
                        var existing = await _unitOfWork.CustomerRepository.GetById(id);
                        return existing != null;
                    }
                    return true;
                })
                .WithMessage("El cliente con ese ID no existe");

            // Business rule: Cannot have both email and phone invalid
            RuleFor(c => c)
                .Must(customer => !string.IsNullOrWhiteSpace(customer.Email) ||
                                 !string.IsNullOrWhiteSpace(customer.Phone))
                .WithMessage("El cliente debe tener al menos un método de contacto válido (email o teléfono)");
        }

        /// <summary>
        /// Valida que el nombre solo contenga letras, espacios y caracteres acentuados
        /// </summary>
        private bool BeAValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;

            // Allow letters, spaces, accented characters
            return Regex.IsMatch(name, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$");
        }

        /// <summary>
        /// Valida que el teléfono contenga solo dígitos, espacios y caracteres permitidos
        /// </summary>
        private bool BeAValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;

            // Allow digits, spaces, +, -, (, )
            return Regex.IsMatch(phone, @"^[\d\s\-\+\(\)]+$");
        }
    }
}
