using ShippingAndLogisticsManagement.Core.Enum;

namespace ShippingAndLogisticsManagement.Core.Entities
{
    public class Vehicle: BaseEntity
    {
        /// <summary>
        /// Número de placa
        /// </summary>
        /// <example>1234-ABC</example>
        public string PlateNumber { get; set; } = null!;

        /// <summary>
        /// Marca del vehículo
        /// </summary>
        /// <example>Toyota</example>
        public string Brand { get; set; } = null!;

        /// <summary>
        /// Modelo
        /// </summary>
        /// <example>Hilux 2020</example>
        public string Model { get; set; } = null!;

        /// <summary>
        /// Tipo de vehículo
        /// </summary>
        public VehicleType Type { get; set; }

        /// <summary>
        /// Capacidad máxima de peso en kg
        /// </summary>
        public double MaxWeightCapacityKg { get; set; }

        /// <summary>
        /// Capacidad en metros cúbicos
        /// </summary>
        public double MaxVolumeCapacityM3 { get; set; }

        /// <summary>
        /// Estado actual del vehículo
        /// </summary>
        public VehicleStatus Status { get; set; } = VehicleStatus.Available;

        /// <summary>
        /// Fecha de último mantenimiento
        /// </summary>
        public DateTime? LastMaintenanceDate { get; set; }

        /// <summary>
        /// Kilometraje actual
        /// </summary>
        public int CurrentMileage { get; set; }

        /// <summary>
        /// ¿Vehículo activo en flota?
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Almacén base del vehículo
        /// </summary>
        public int? BaseWarehouseId { get; set; }

        /// <summary>
        /// Conductor asignado actualmente (opcional)
        /// </summary>
        public int? AssignedDriverId { get; set; }

        public Warehouse? BaseWarehouse { get; set; }
        public Driver? AssignedDriver { get; set; }
    }
}
