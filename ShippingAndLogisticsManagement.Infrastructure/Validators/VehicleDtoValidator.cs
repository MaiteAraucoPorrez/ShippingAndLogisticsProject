using FluentValidation;
using ShippingAndLogisticsManagement.Core.Enum;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;
using System.Text.RegularExpressions;

namespace ShippingAndLogisticsManagement.Infrastructure.Validators
{
    /// <summary>
    /// Validador para VehicleDto con reglas de negocio completas
    /// </summary>
    public class VehicleDtoValidator : AbstractValidator<VehicleDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        private static readonly Regex PlateRegex = new(@"^[A-Z0-9\-]+$");
        private static readonly Regex VinRegex = new(@"^[A-HJ-NPR-Z0-9]{17}$");

        public VehicleDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(v => v.PlateNumber)
                .NotEmpty()
                .WithMessage("La placa es requerida")
                .MinimumLength(5)
                .WithMessage("La placa debe tener al menos 5 caracteres")
                .MaximumLength(20)
                .WithMessage("La placa no puede exceder 20 caracteres")
                .Must(BeAValidPlateNumber)
                .WithMessage("La placa solo puede contener letras, números y guiones")
                .MustAsync(async (dto, plateNumber, ct) =>
                {
                    var existing = await _unitOfWork.VehicleRepository
                        .GetByPlateNumberAsync(plateNumber);

                    // Si es creación (Id==0), la placa no debe existir
                    // Si es actualización, la placa puede existir solo si es el mismo vehículo
                    return existing == null || (dto.Id != 0 && existing.Id == dto.Id);
                })
                .WithMessage("Ya existe un vehículo con esa placa");


            RuleFor(v => v.Type)
                .IsInEnum().WithMessage("El tipo de vehículo no es válido");

            RuleFor(v => v.MaxWeightCapacityKg)
                .GreaterThan(0).WithMessage("La capacidad de peso debe ser mayor a 0 kg")
                .LessThanOrEqualTo(50000).WithMessage("La capacidad de peso no puede exceder 50,000 kg")
                // Validación cruzada: Tipo vs Capacidad
                .Must((dto, capacity) => ValidateCapacityByType(dto.Type, capacity))
                .WithMessage(dto => GetCapacityRangeMessage(dto.Type));

            RuleFor(v => v.MaxVolumeCapacityM3)
                .NotEmpty()
                .WithMessage("La capacidad de volumen es requerida")
                .GreaterThan(0)
                .WithMessage("La capacidad de volumen debe ser mayor a 0 m³")
                .LessThanOrEqualTo(200)
                .WithMessage("La capacidad de volumen no puede exceder 200 m³")
                .Must(BeValidCapacity)
                .WithMessage("La capacidad de volumen debe tener máximo 2 decimales");

            RuleFor(v => v.Status)
                .IsInEnum().WithMessage("El estado no es válido");

            RuleFor(v => v.VIN)
                .MaximumLength(17)
                .When(v => !string.IsNullOrWhiteSpace(v.VIN))
                .WithMessage("El VIN no puede exceder 17 caracteres")
                .Must(BeAValidVIN)
                .When(v => !string.IsNullOrWhiteSpace(v.VIN))
                .WithMessage("El VIN solo puede contener letras mayúsculas y números (sin I, O, Q)")
                .MustAsync(async (dto, vin, ct) =>
                {
                    if (string.IsNullOrWhiteSpace(vin)) return true;

                    var vehicles = await _unitOfWork.VehicleRepository.GetAll();
                    var existing = vehicles.FirstOrDefault(v => v.VIN != null && v.VIN.Equals(vin, StringComparison.OrdinalIgnoreCase));
                    return existing == null || (dto.Id != 0 && existing.Id == dto.Id);
                })
                .When(v => !string.IsNullOrWhiteSpace(v.VIN))
                .WithMessage("Ya existe un vehículo con ese VIN");

            RuleFor(v => v.BaseWarehouseId)
                .MustAsync(async (warehouseId, ct) =>
                {
                    if (!warehouseId.HasValue) return true;

                    var warehouse = await _unitOfWork.WarehouseRepository.GetById(warehouseId.Value);
                    return warehouse != null && warehouse.IsActive;
                })
                .When(v => v.BaseWarehouseId.HasValue)
                .WithMessage("El almacén base no existe o está inactivo");

            RuleFor(v => v.AssignedDriverId)
                .MustAsync(async (dto, driverId, ct) =>
                {
                    if (!driverId.HasValue) return true;

                    var driver = await _unitOfWork.DriverRepository.GetById(driverId.Value);
                    if (driver == null || !driver.IsActive) return false;

                    // Si el conductor ya tiene vehículo y NO es este mismo vehículo (en edición)
                    if (driver.CurrentVehicleId != null && driver.CurrentVehicleId != dto.Id)
                    {
                        return false;
                    }
                    return true;
                })
                .When(v => v.AssignedDriverId.HasValue)
                .WithMessage("El conductor no existe, está inactivo o ya tiene otro vehículo asignado");

            RuleFor(v => v.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El ID no puede ser negativo")
                .MustAsync(async (id, ct) =>
                {
                    if (id > 0)
                    {
                        var existing = await _unitOfWork.VehicleRepository.GetById(id);
                        return existing != null;
                    }
                    return true;
                })
                .WithMessage("El vehículo con ese ID no existe");

            // Regla: Si está InTransit, debe tener conductor asignado
            RuleFor(v => v)
                .Must(v => !(v.Status == VehicleStatus.InTransit && !v.AssignedDriverId.HasValue))
                .WithMessage("Un vehículo 'En Tránsito' debe tener un conductor asignado");

            // Regla: Capacidad coherente peso/volumen
            RuleFor(v => v)
                .Must(v => ValidateWeightVolumeRatio(v.MaxWeightCapacityKg, v.MaxVolumeCapacityM3))
                .WithMessage("La relación peso/volumen parece inusual. Verifique los datos");
        }

        /// <summary>
        /// Valida formato de placa (letras, números, guiones)
        /// </summary>
        private bool BeAValidPlateNumber(string plateNumber)
        {
            if (string.IsNullOrWhiteSpace(plateNumber)) return false;
            return Regex.IsMatch(plateNumber, @"^[A-Z0-9\-]+$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Valida que las capacidades tengan máximo 2 decimales
        /// </summary>
        private bool BeValidCapacity(double capacity)
        {
            var decimalPlaces = BitConverter.GetBytes(decimal.GetBits((decimal)capacity)[3])[2];
            return decimalPlaces <= 2;
        }

        /// <summary>
        /// Valida formato VIN (17 caracteres sin I, O, Q)
        /// </summary>
        private bool BeAValidVIN(string vin)
        {
            if (string.IsNullOrWhiteSpace(vin)) return false;

            // VIN debe tener exactamente 17 caracteres
            if (vin.Length != 17) return false;

            // VIN no puede contener I, O, Q
            return Regex.IsMatch(vin, @"^[A-HJ-NPR-Z0-9]{17}$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Valida capacidad según el tipo de vehículo
        /// </summary>
        private bool ValidateCapacityByType(VehicleType type, double capacity)
        {
            return type switch
            {
                VehicleType.Motorcycle => capacity <= 300,
                VehicleType.Van => capacity <= 3000,
                VehicleType.Pickup => capacity <= 5000,
                VehicleType.Truck => capacity <= 50000,
                _ => true
            };
        }

        /// <summary>
        /// Obtiene el mensaje de rango de capacidad según tipo
        /// </summary>
        private string GetCapacityRangeMessage(VehicleType type)
        {
            return type switch
            {
                VehicleType.Motorcycle => "Una motocicleta no puede superar los 300 kg",
                VehicleType.Van => "Una van no puede superar los 3,000 kg",
                VehicleType.Pickup => "Una pickup no puede superar los 5,000 kg",
                VehicleType.Truck => "Un camión no puede superar los 50,000 kg",
                _ => "Capacidad inválida"
            };
        }

        /// <summary>
        /// Valida relación peso/volumen coherente
        /// </summary>
        private bool ValidateWeightVolumeRatio(double weight, double volume)
        {
            if (volume <= 0) return true; // Evita división por cero
            var density = weight / volume;
            // Rango ampliado: 20 kg/m3 a 2000 kg/m3
            return density >= 20 && density <= 2000;
        }
    }
}