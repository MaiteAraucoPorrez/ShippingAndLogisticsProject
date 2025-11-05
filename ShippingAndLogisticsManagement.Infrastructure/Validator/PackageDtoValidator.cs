using FluentValidation;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;

namespace ShippingAndLogisticsManagement.Infrastructure.Validator
{
    public class PackageDtoValidator : AbstractValidator<PackageDto>
    {
        public PackageDtoValidator()
        {
            RuleFor(p => p.Description)
                .NotEmpty().WithMessage("La descripcion es requerida.")
                .MaximumLength(100);

            RuleFor(p => p.Weight)
                .GreaterThan(0).WithMessage("El peso debe ser mayor a cero.");

            RuleFor(p => p.Price)
                .GreaterThanOrEqualTo(0).WithMessage("El precio no puede ser negativo.");

            RuleFor(p => p.ShipmentId)
                .GreaterThan(0).WithMessage("El ID del envio es requerido.");
        }
    }
}
