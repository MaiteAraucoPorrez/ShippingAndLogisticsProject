using FluentValidation;
using ShippingAndLogisticsManagement.Core.CustomEntities;

namespace ShippingAndLogisticsManagement.Infrastructure.Validator
{
    public class GetByIdRequestValidator : AbstractValidator<GetByIdRequest>
    {
        public GetByIdRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("El ID es requerido")
                .GreaterThan(0).WithMessage("El ID debe ser mayor a 0")
                .LessThanOrEqualTo(1000000).WithMessage("El ID no puede ser mayor a 1,000,000")
                .Must(BeAValidIdFormat).WithMessage("El ID debe ser un número válido");
        }
        private static bool BeAValidIdFormat(int id)
        {
            // Validaciones adicionales para el formato del ID
            return id.ToString().Length <= 7; // Máximo 7 dígitos
        }
    }
}

