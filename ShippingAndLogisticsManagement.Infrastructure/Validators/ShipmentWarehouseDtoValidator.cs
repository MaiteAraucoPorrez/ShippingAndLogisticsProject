using FluentValidation;
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
                .NotEmpty()
                .WithMessage("El ID del almacén es requerido")
                .GreaterThan(0)
                .WithMessage("El ID del almacén debe ser mayor a 0")
                .MustAsync(async (warehouseId, ct) =>
                {
                    var warehouse = await _unitOfWork.WarehouseRepository.GetById(warehouseId);
                    return warehouse != null;
                })
                .WithMessage("El almacén asociado no existe")
                .MustAsync(async (warehouseId, ct) =>
                {
                    var warehouse = await _unitOfWork.WarehouseRepository.GetById(warehouseId);
                    return warehouse != null && warehouse.IsActive;
                })
                .WithMessage("El almacén no está activo");

            // EntryDate validation
            RuleFor(sw => sw.EntryDate)
                .NotEmpty()
                .WithMessage("La fecha de entrada es requerida")
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("La fecha de entrada no puede ser futura");

            // ExitDate validation (optional)
            RuleFor(sw => sw.ExitDate)
                .GreaterThan(sw => sw.EntryDate)
                .When(sw => sw.ExitDate.HasValue)
                .WithMessage("La fecha de salida debe ser posterior a la fecha de entrada")
                .LessThanOrEqualTo(DateTime.Now)
                .When(sw => sw.ExitDate.HasValue)
                .WithMessage("La fecha de salida no puede ser futura");

            // Status validation
            RuleFor(sw => sw.Status)
                .NotEmpty()
                .WithMessage("El estado es requerido")
                .Must(BeAValidStatus)
                .WithMessage("El estado debe ser 'Received', 'InStorage', 'Processing' o 'Dispatched'");

            // ReceivedBy validation (optional)
            RuleFor(sw => sw.ReceivedBy)
                .MaximumLength(100)
                .When(sw => !string.IsNullOrWhiteSpace(sw.ReceivedBy))
                .WithMessage("El nombre de quien recibió no puede exceder 100 caracteres");

            // DispatchedBy validation (optional)
            RuleFor(sw => sw.DispatchedBy)
                .MaximumLength(100)
                .When(sw => !string.IsNullOrWhiteSpace(sw.DispatchedBy))
                .WithMessage("El nombre de quien despachó no puede exceder 100 caracteres");

            // Notes validation (optional)
            RuleFor(sw => sw.Notes)
                .MaximumLength(500)
                .When(sw => !string.IsNullOrWhiteSpace(sw.Notes))
                .WithMessage("Las notas no pueden exceder 500 caracteres");

            // StorageLocation validation (optional)
            RuleFor(sw => sw.StorageLocation)
                .MaximumLength(50)
                .When(sw => !string.IsNullOrWhiteSpace(sw.StorageLocation))
                .WithMessage("La ubicación de almacenamiento no puede exceder 50 caracteres");

            // Business rule: Si el estado es "Dispatched", debe tener ExitDate
            RuleFor(sw => sw)
                .Must(sw => sw.Status != "Dispatched" || sw.ExitDate.HasValue)
                .WithMessage("Si el estado es 'Dispatched', debe especificar la fecha de salida");

            // Business rule: Si tiene ExitDate, el estado debe ser "Dispatched"
            RuleFor(sw => sw)
                .Must(sw => !sw.ExitDate.HasValue || sw.Status == "Dispatched")
                .WithMessage("Si especifica fecha de salida, el estado debe ser 'Dispatched'");

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

        private bool BeAValidStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status)) return false;

            var validStatuses = new[] { "Received", "InStorage", "Processing", "Dispatched" };
            return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
        }
    }
}