using FluentValidation;
using ShippingAndLogisticsManagement.Core.Enum;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;

namespace ShippingAndLogisticsManagement.Infrastructure.Validators
{
    public class ShipmentWarehouseDtoValidator : AbstractValidator<ShipmentWarehouseDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShipmentWarehouseDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // ShipmentId validation
            RuleFor(sw => sw.ShipmentId)
                .NotEmpty()
                .WithMessage("El ID del envío es requerido")
                .GreaterThan(0)
                .WithMessage("El ID del envío debe ser mayor a 0")
                .MustAsync(async (shipmentId, ct) =>
                {
                    var shipment = await _unitOfWork.ShipmentRepository.GetById(shipmentId);
                    return shipment != null;
                })
                .WithMessage("El envío asociado no existe");

            // WarehouseId validation
            RuleFor(sw => sw.WarehouseId)
                .GreaterThan(0).WithMessage("El ID del almacén es inválido")
                .MustAsync(async (warehouseId, ct) =>
                {
                    var warehouse = await _unitOfWork.WarehouseRepository.GetById(warehouseId);
                    return warehouse != null && warehouse.IsActive;
                })
                .WithMessage("El almacén no existe o está inactivo");

            // EntryDate validation
            RuleFor(sw => sw.EntryDate)
                .NotEmpty().WithMessage("La fecha de entrada es requerida")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de entrada no puede ser futura");

            // ExitDate validation
            RuleFor(sw => sw.ExitDate)
                .GreaterThan(sw => sw.EntryDate)
                .When(sw => sw.ExitDate.HasValue)
                .WithMessage("La fecha de salida debe ser posterior a la fecha de entrada")
                .LessThanOrEqualTo(DateTime.Now)
                .When(sw => sw.ExitDate.HasValue)
                .WithMessage("La fecha de salida no puede ser futura");

            // Status validation
            RuleFor(sw => sw.Status)
                .IsInEnum().WithMessage("El estado no es válido");

            // ReceivedBy validation
            RuleFor(sw => sw.ReceivedBy)
                .MaximumLength(100)
                .When(sw => !string.IsNullOrWhiteSpace(sw.ReceivedBy))
                .WithMessage("El nombre de quien recibió no puede exceder 100 caracteres");

            // DispatchedBy validation
            RuleFor(sw => sw.DispatchedBy)
                .MaximumLength(100)
                .When(sw => !string.IsNullOrWhiteSpace(sw.DispatchedBy))
                .WithMessage("El nombre de quien despachó no puede exceder 100 caracteres");

            // StorageLocation validation (optional)
            RuleFor(sw => sw.StorageLocation)
                .MaximumLength(50)
                .When(sw => !string.IsNullOrWhiteSpace(sw.StorageLocation))
                .WithMessage("La ubicación de almacenamiento no puede exceder 50 caracteres");

            // Business rule: Si el estado es "Dispatched", debe tener ExitDate
            RuleFor(sw => sw)
                .Must(sw => sw.Status == WarehouseShipmentStatus.Dispatched || !sw.ExitDate.HasValue)
                .WithMessage("Solo los envíos despachados ('Dispatched') pueden tener fecha de salida");

            // Business rule: Si tiene ExitDate, el estado debe ser "Dispatched"
            RuleFor(sw => sw)
                 .Must(sw => sw.Status != WarehouseShipmentStatus.Dispatched || sw.ExitDate.HasValue)
                 .WithMessage("Si el estado es 'Dispatched', debe especificar la fecha de salida");

            // ID validation (for updates)
            RuleFor(sw => sw.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El ID no puede ser negativo")
                .MustAsync(async (dto, id, ct) =>
                {
                    if (id > 0)
                    {
                        var existing = await _unitOfWork.ShipmentWarehouseRepository.GetById(id);
                        return existing != null;
                    }
                    return true;
                })
                .WithMessage("El registro de envío-almacén con ese ID no existe");
        }
    }
}