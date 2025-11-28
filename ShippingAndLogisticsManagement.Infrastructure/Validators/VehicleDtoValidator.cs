using FluentValidation;
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

        // Tipos de vehículo válidos
        private static readonly string[] ValidVehicleTypes = new[]
        {
            "Motorcycle", "Van", "Truck", "Pickup"
        };

        // Estados válidos
        private static readonly string[] ValidVehicleStatuses = new[]
        {
            "Available", "InTransit", "UnderMaintenance", "OutOfService"
        };

        public VehicleDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            // ========== PlateNumber Validation ==========
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

            // ========== Brand Validation ==========
            RuleFor(v => v.Brand)
                .NotEmpty()
                .WithMessage("La marca es requerida")
                .MinimumLength(2)
                .WithMessage("La marca debe tener al menos 2 caracteres")
                .MaximumLength(50)
                .WithMessage("La marca no puede exceder 50 caracteres")
                .Must(BeAValidBrand)
                .WithMessage("La marca solo puede contener letras, números y espacios");

            // ========== Model Validation ==========
            RuleFor(v => v.Model)
                .NotEmpty()
                .WithMessage("El modelo es requerido")
                .MinimumLength(2)
                .WithMessage("El modelo debe tener al menos 2 caracteres")
                .MaximumLength(100)
                .WithMessage("El modelo no puede exceder 100 caracteres")
                .Must(BeAValidModel)
                .WithMessage("El modelo contiene caracteres no permitidos");

            // ========== Type Validation ==========
            RuleFor(v => v.Type)
                .NotEmpty()
                .WithMessage("El tipo de vehículo es requerido")
                .Must(BeAValidVehicleType)
                .WithMessage(v => $"El tipo '{v.Type}' no es válido. " +
                    $"Debe ser uno de: {string.Join(", ", ValidVehicleTypes)}");

            // ========== MaxWeightCapacityKg Validation ==========
            RuleFor(v => v.MaxWeightCapacityKg)
                .NotEmpty()
                .WithMessage("La capacidad de peso es requerida")
                .GreaterThan(0)
                .WithMessage("La capacidad de peso debe ser mayor a 0 kg")
                .LessThanOrEqualTo(50000)
                .WithMessage("La capacidad de peso no puede exceder 50,000 kg")
                .Must(BeValidCapacity)
                .WithMessage("La capacidad de peso debe tener máximo 2 decimales")
                .Must((dto, capacity) => ValidateCapacityByType(dto.Type, capacity))
                .WithMessage(dto => GetCapacityRangeMessage(dto.Type));

            // ========== MaxVolumeCapacityM3 Validation ==========
            RuleFor(v => v.MaxVolumeCapacityM3)
                .NotEmpty()
                .WithMessage("La capacidad de volumen es requerida")
                .GreaterThan(0)
                .WithMessage("La capacidad de volumen debe ser mayor a 0 m³")
                .LessThanOrEqualTo(200)
                .WithMessage("La capacidad de volumen no puede exceder 200 m³")
                .Must(BeValidCapacity)
                .WithMessage("La capacidad de volumen debe tener máximo 2 decimales");

            // ========== Status Validation ==========
            RuleFor(v => v.Status)
                .NotEmpty()
                .WithMessage("El estado es requerido")
                .Must(BeAValidStatus)
                .WithMessage(v => $"El estado '{v.Status}' no es válido. " +
                    $"Debe ser uno de: {string.Join(", ", ValidVehicleStatuses)}");

            // ========== VIN Validation (opcional) ==========
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

            // ========== Year Validation (opcional) ==========
            RuleFor(v => v.Year)
                .InclusiveBetween(1900, DateTime.Now.Year + 1)
                .When(v => v.Year != 0)
                .WithMessage($"El año debe estar entre 1900 y {DateTime.Now.Year + 1}");

            // ========== CurrentMileage Validation ==========
            RuleFor(v => v.CurrentMileage)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El kilometraje no puede ser negativo")
                .LessThanOrEqualTo(5000000)
                .WithMessage("El kilometraje no puede exceder 5,000,000 km");

            // ========== LastMaintenanceDate Validation (opcional) ==========
            RuleFor(v => v.LastMaintenanceDate)
                .LessThanOrEqualTo(DateTime.Today)
                .When(v => v.LastMaintenanceDate.HasValue)
                .WithMessage("La fecha de último mantenimiento no puede ser futura");

            // ========== InsuranceExpiryDate Validation (opcional) ==========
            RuleFor(v => v.InsuranceExpiryDate)
                .GreaterThan(DateTime.Today)
                .When(v => v.InsuranceExpiryDate.HasValue)
                .WithMessage("La fecha de vencimiento del seguro debe ser futura");

            // ========== BaseWarehouseId Validation (opcional) ==========
            RuleFor(v => v.BaseWarehouseId)
                .MustAsync(async (warehouseId, ct) =>
                {
                    if (!warehouseId.HasValue) return true;

                    var warehouse = await _unitOfWork.WarehouseRepository.GetById(warehouseId.Value);
                    return warehouse != null && warehouse.IsActive;
                })
                .When(v => v.BaseWarehouseId.HasValue)
                .WithMessage("El almacén base no existe o está inactivo");

            // ========== AssignedDriverId Validation (opcional) ==========
            RuleFor(v => v.AssignedDriverId)
                .MustAsync(async (driverId, ct) =>
                {
                    if (!driverId.HasValue) return true;

                    var driver = await _unitOfWork.DriverRepository.GetById(driverId.Value);
                    return driver != null && driver.IsActive;
                })
                .When(v => v.AssignedDriverId.HasValue)
                .WithMessage("El conductor asignado no existe o está inactivo")
                .MustAsync(async (dto, driverId, ct) =>
                {
                    if (!driverId.HasValue) return true;

                    // Verificar que el conductor no esté asignado a otro vehículo
                    var driver = await _unitOfWork.DriverRepository.GetById(driverId.Value);
                    if (driver?.CurrentVehicleId != null)
                    {
                        // Si es actualización y es el mismo vehículo, permitir
                        return dto.Id != 0 && driver.CurrentVehicleId == dto.Id;
                    }
                    return true;
                })
                .When(v => v.AssignedDriverId.HasValue)
                .WithMessage("El conductor ya está asignado a otro vehículo");

            // ========== ID Validation (para actualizaciones) ==========
            RuleFor(v => v.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El ID no puede ser negativo")
                .MustAsync(async (dto, id, ct) =>
                {
                    if (id > 0)
                    {
                        var existing = await _unitOfWork.VehicleRepository.GetById(id);
                        return existing != null;
                    }
                    return true;
                })
                .WithMessage("El vehículo con ese ID no existe");

            // ========== Business Rules ==========

            // Regla: Si está InTransit, debe tener conductor asignado
            RuleFor(v => v)
                .Must(v => !(v.Status == "InTransit" && !v.AssignedDriverId.HasValue))
                .WithMessage("Un vehículo 'En Tránsito' debe tener un conductor asignado");

            // Regla: Si está bajo mantenimiento, no puede estar en tránsito
            RuleFor(v => v.Status)
                .NotEqual("InTransit")
                .When(v => v.Status == "UnderMaintenance")
                .WithMessage("Un vehículo en mantenimiento no puede estar en tránsito");

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
        /// Valida formato de marca
        /// </summary>
        private bool BeAValidBrand(string brand)
        {
            if (string.IsNullOrWhiteSpace(brand)) return false;
            return Regex.IsMatch(brand, @"^[a-zA-Z0-9\s]+$");
        }

        /// <summary>
        /// Valida formato de modelo
        /// </summary>
        private bool BeAValidModel(string model)
        {
            if (string.IsNullOrWhiteSpace(model)) return false;
            return Regex.IsMatch(model, @"^[a-zA-Z0-9\s\-.,()]+$");
        }

        /// <summary>
        /// Valida que el tipo sea uno de los permitidos
        /// </summary>
        private bool BeAValidVehicleType(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) return false;
            return ValidVehicleTypes.Contains(type, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Valida que el estado sea uno de los permitidos
        /// </summary>
        private bool BeAValidStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status)) return false;
            return ValidVehicleStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
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
        private bool ValidateCapacityByType(string type, double capacity)
        {
            return type?.ToLower() switch
            {
                "motorcycle" => capacity <= 300,      // Máximo 300 kg
                "van" => capacity <= 3000,            // Máximo 3,000 kg
                "pickup" => capacity <= 5000,         // Máximo 5,000 kg
                "truck" => capacity <= 50000,         // Máximo 50,000 kg
                _ => true
            };
        }

        /// <summary>
        /// Obtiene el mensaje de rango de capacidad según tipo
        /// </summary>
        private string GetCapacityRangeMessage(string type)
        {
            return type?.ToLower() switch
            {
                "motorcycle" => "Una motocicleta no puede tener capacidad mayor a 300 kg",
                "van" => "Una van no puede tener capacidad mayor a 3,000 kg",
                "pickup" => "Una pickup no puede tener capacidad mayor a 5,000 kg",
                "truck" => "Un camión no puede tener capacidad mayor a 50,000 kg",
                _ => "La capacidad no es válida para este tipo de vehículo"
            };
        }

        /// <summary>
        /// Valida relación peso/volumen coherente
        /// </summary>
        private bool ValidateWeightVolumeRatio(double weight, double volume)
        {
            if (volume <= 0) return true;

            // Densidad típica: 100-500 kg/m³
            var density = weight / volume;
            return density >= 50 && density <= 1000;
        }
    }
}