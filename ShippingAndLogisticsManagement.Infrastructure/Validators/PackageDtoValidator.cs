using FluentValidation;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;

namespace ShippingAndLogisticsManagement.Infrastructure.Validators
{
    public class PackageDtoValidator : AbstractValidator<PackageDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public PackageDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(p => p.Description)
                .NotEmpty()
                .WithMessage("La descripción es requerida")
                .MinimumLength(3)
                .WithMessage("La descripción debe tener al menos 3 caracteres")
                .MaximumLength(200)
                .WithMessage("La descripción no puede exceder 200 caracteres")
                .Must(BeAValidDescription)
                .WithMessage("La descripción solo puede contener letras, números, espacios y signos de puntuación básicos");

            
            RuleFor(p => p.Weight)
                .NotEmpty()
                .WithMessage("El peso es requerido")
                .GreaterThan(0.1)
                .WithMessage("El peso debe ser mayor a 0.1 kg")
                .LessThanOrEqualTo(100)
                .WithMessage("El peso no puede exceder 100 kg")
                .Must(BeValidWeight)
                .WithMessage("El peso debe tener máximo 2 decimales");

      
            RuleFor(p => p.Price)
                .NotEmpty()
                .WithMessage("El precio es requerido")
                .GreaterThan(0)
                .WithMessage("El precio debe ser mayor a 0")
                .LessThanOrEqualTo(1000000)
                .WithMessage("El precio no puede exceder 1,000,000")
                .Must(BeValidPrice)
                .WithMessage("El precio debe tener máximo 2 decimales");


            RuleFor(p => p.ShipmentId)
                .NotEmpty()
                .WithMessage("El ID del envío es requerido")
                .GreaterThan(0)
                .WithMessage("El ID del envío debe ser mayor a 0")
                .MustAsync(async (shipmentId, ct) =>
                {
                    var shipment = await _unitOfWork.ShipmentRepository.GetById(shipmentId);
                    return shipment != null;
                })
                .WithMessage("El envío asociado no existe")
                .MustAsync(async (dto, shipmentId, ct) =>
                {
                    var shipment = await _unitOfWork.ShipmentRepository.GetById(shipmentId);

                    if (dto.Id == 0 || shipment?.Id != shipmentId)
                    {
                        return shipment != null && shipment.State != "Delivered";
                    }
                    return true;
                })
                .WithMessage("No se pueden agregar/mover paquetes a un envío entregado")
                .MustAsync(async (shipmentId, ct) =>
                {
                    var existingPackages = await _unitOfWork.PackageRepository
                        .GetByShipmentIdDapperAsync(shipmentId);
                    return existingPackages == null || existingPackages.Count() < 50;
                })
                .WithMessage("El envío alcanzó el límite máximo de 50 paquetes");


            RuleFor(p => p.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El ID no puede ser negativo")
                .MustAsync(async (dto, id, ct) =>
                {
                    // Si es actualización (Id > 0), verificar que exista
                    if (id > 0)
                    {
                        var existing = await _unitOfWork.PackageRepository.GetById(id);
                        return existing != null;
                    }
                    return true;
                })
                .WithMessage("El paquete con ese ID no existe")
                .MustAsync(async (dto, id, ct) =>
                {
                    // Si es actualización, verificar que el envío original no esté entregado
                    if (id > 0)
                    {
                        var existing = await _unitOfWork.PackageRepository.GetById(id);
                        if (existing != null)
                        {
                            var shipment = await _unitOfWork.ShipmentRepository.GetById(existing.ShipmentId);
                            return shipment == null || shipment.State != "Delivered";
                        }
                    }
                    return true;
                })
                .WithMessage("No se puede modificar un paquete de un envío entregado");

            RuleFor(p => p)
                .Must(package => ValidateWeightPriceRatio(package.Weight, package.Price))
                .WithMessage("La relación peso/precio parece inusual. Verifique los datos");
        }


        /// <summary>
        /// Valida que la descripción solo contenga caracteres permitidos
        /// </summary>
        private bool BeAValidDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description)) return false;

            // Permitir letras, números, espacios, guiones, comas, puntos y paréntesis
            return System.Text.RegularExpressions.Regex.IsMatch(
                description,
                @"^[a-zA-Z0-9\s\-.,()áéíóúÁÉÍÓÚñÑüÜ]+$"
            );
        }

        /// <summary>
        /// Valida que el peso tenga máximo 2 decimales
        /// </summary>
        private bool BeValidWeight(double weight)
        {
            var decimalPlaces = BitConverter.GetBytes(decimal.GetBits((decimal)weight)[3])[2];
            return decimalPlaces <= 2;
        }

        /// <summary>
        /// Valida que el precio tenga máximo 2 decimales
        /// </summary>
        private bool BeValidPrice(decimal price)
        {
            var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(price)[3])[2];
            return decimalPlaces <= 2;
        }

        /// <summary>
        /// Validación de lógica de negocio: relación peso-precio razonable
        /// Alerta si el precio por kg es muy alto o muy bajo
        /// </summary>
        private bool ValidateWeightPriceRatio(double weight, decimal price)
        {
            if (weight <= 0 || price <= 0) return true;

            var pricePerKg = (double)price / weight;

            // Alertar si el precio por kg es menor a 1 o mayor a 100,000
            return pricePerKg >= 1 && pricePerKg <= 100000;
        }
    }
}