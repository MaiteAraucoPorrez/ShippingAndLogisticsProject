namespace ShippingAndLogisticsManagement.Core.Entities
{
    /// <summary>
    /// Representa un vehículo de transporte en la flota logística
    /// </summary>
    /// <remarks>
    /// Los vehículos son asignados para transportar envíos entre almacenes
    /// y hasta los destinos finales. Cada vehículo tiene capacidad limitada
    /// tanto en peso como en volumen.
    /// 
    /// Tipos de vehículos:
    /// - Motorcycle: Para paquetes pequeños y entregas rápidas urbanas
    /// - Van: Para entregas medianas en ciudad
    /// - Pickup: Para cargas medianas/pesadas
    /// - Truck: Para grandes volúmenes y largas distancias
    /// 
    /// Reglas de negocio:
    /// - Número de placa único por vehículo
    /// - Solo vehículos disponibles pueden asignarse a rutas
    /// - Debe tener mantenimiento periódico
    /// - Capacidad de peso y volumen mayores a 0
    /// </remarks>
    public class Vehicle : BaseEntity
    {
        /// <summary>
        /// Identificador único del vehículo
        /// </summary>
        /// <example>1</example>
        // public int Id { get; set; } // Heredado de BaseEntity

        /// <summary>
        /// Número de placa del vehículo (único)
        /// </summary>
        /// <remarks>
        /// Debe ser único en el sistema. Formato común en Bolivia: 1234-ABC
        /// </remarks>
        /// <example>1234-ABC</example>
        public string PlateNumber { get; set; } = null!;

        /// <summary>
        /// Marca del vehículo
        /// </summary>
        /// <example>Toyota</example>
        public string Brand { get; set; } = null!;

        /// <summary>
        /// Modelo del vehículo con año
        /// </summary>
        /// <example>Hilux 2020</example>
        public string Model { get; set; } = null!;

        /// <summary>
        /// Año de fabricación
        /// </summary>
        /// <example>2020</example>
        public int Year { get; set; }

        /// <summary>
        /// Color del vehículo
        /// </summary>
        /// <example>Blanco</example>
        public string? Color { get; set; }

        /// <summary>
        /// Tipo de vehículo según su categoría
        /// </summary>
        /// <example>Truck</example>
        public VehicleType Type { get; set; }

        /// <summary>
        /// Capacidad máxima de peso en kilogramos
        /// </summary>
        /// <remarks>
        /// No se pueden asignar envíos que excedan esta capacidad.
        /// </remarks>
        /// <example>3500.0</example>
        public double MaxWeightCapacityKg { get; set; }

        /// <summary>
        /// Capacidad máxima de volumen en metros cúbicos
        /// </summary>
        /// <remarks>
        /// Útil para envíos voluminosos pero ligeros.
        /// </remarks>
        /// <example>15.5</example>
        public double MaxVolumeCapacityM3 { get; set; }

        /// <summary>
        /// Peso actual transportado en kg
        /// </summary>
        /// <remarks>
        /// Se actualiza al asignar/descargar envíos.
        /// No puede exceder MaxWeightCapacityKg.
        /// </remarks>
        /// <example>1200.0</example>
        public double CurrentWeightKg { get; set; }

        /// <summary>
        /// Volumen actual ocupado en m³
        /// </summary>
        /// <remarks>
        /// Se actualiza al asignar/descargar envíos.
        /// No puede exceder MaxVolumeCapacityM3.
        /// </remarks>
        /// <example>8.5</example>
        public double CurrentVolumeM3 { get; set; }

        /// <summary>
        /// Estado actual del vehículo
        /// </summary>
        /// <example>Available</example>
        public VehicleStatus Status { get; set; } = VehicleStatus.Available;

        /// <summary>
        /// Fecha de último mantenimiento
        /// </summary>
        /// <example>2024-12-15</example>
        public DateTime? LastMaintenanceDate { get; set; }

        /// <summary>
        /// Fecha del próximo mantenimiento programado
        /// </summary>
        /// <example>2025-03-15</example>
        public DateTime? NextMaintenanceDate { get; set; }

        /// <summary>
        /// Kilometraje actual del vehículo
        /// </summary>
        /// <example>85000</example>
        public int CurrentMileage { get; set; }

        /// <summary>
        /// Kilometraje del último mantenimiento
        /// </summary>
        /// <example>80000</example>
        public int? LastMaintenanceMileage { get; set; }

        /// <summary>
        /// Tipo de combustible que usa
        /// </summary>
        /// <example>Diesel</example>
        public FuelType? FuelType { get; set; }

        /// <summary>
        /// Consumo promedio en litros por 100 km
        /// </summary>
        /// <example>12.5</example>
        public double? FuelConsumptionPer100Km { get; set; }

        /// <summary>
        /// Número de identificación del vehículo (VIN)
        /// </summary>
        /// <example>1HGBH41JXMN109186</example>
        public string? VIN { get; set; }

        /// <summary>
        /// Número de póliza del seguro
        /// </summary>
        /// <example>POL-2024-12345</example>
        public string? InsurancePolicyNumber { get; set; }

        /// <summary>
        /// Fecha de vencimiento del seguro
        /// </summary>
        /// <example>2025-06-30</example>
        public DateTime? InsuranceExpiryDate { get; set; }

        /// <summary>
        /// Indica si el vehículo está activo en la flota
        /// </summary>
        /// <remarks>
        /// Vehículos inactivos no pueden ser asignados a rutas.
        /// </remarks>
        /// <example>true</example>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// ID del almacén base donde opera el vehículo
        /// </summary>
        /// <remarks>
        /// Opcional. Indica a qué almacén pertenece el vehículo.
        /// </remarks>
        /// <example>2</example>
        public int? BaseWarehouseId { get; set; }

        /// <summary>
        /// ID del conductor actualmente asignado
        /// </summary>
        /// <remarks>
        /// Null si el vehículo no tiene conductor asignado.
        /// </remarks>
        /// <example>5</example>
        public int? AssignedDriverId { get; set; }

        /// <summary>
        /// Fecha de adquisición del vehículo
        /// </summary>
        /// <example>2020-03-15</example>
        public DateTime PurchaseDate { get; set; }

        /// <summary>
        /// Fecha de registro en el sistema
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Notas adicionales sobre el vehículo
        /// </summary>
        /// <example>Tiene GPS instalado, requiere mantenimiento especial</example>
        public string? Notes { get; set; }

        /// <summary>
        /// Navegación al almacén base
        /// </summary>
        public Warehouse? BaseWarehouse { get; set; }

        /// <summary>
        /// Navegación al conductor asignado
        /// </summary>
        public Driver? AssignedDriver { get; set; }
    }

    /// <summary>
    /// Tipo de vehículo según su categoría y uso
    /// </summary>
    public enum VehicleType
    {
        /// <summary>
        /// Motocicleta - Para entregas rápidas urbanas
        /// </summary>
        Motorcycle = 1,

        /// <summary>
        /// Camioneta - Para entregas medianas
        /// </summary>
        Van = 2,

        /// <summary>
        /// Pickup - Para cargas medianas/pesadas
        /// </summary>
        Pickup = 3,

        /// <summary>
        /// Camión - Para grandes volúmenes
        /// </summary>
        Truck = 4
    }

    /// <summary>
    /// Estado operativo del vehículo
    /// </summary>
    public enum VehicleStatus
    {
        /// <summary>
        /// Disponible para asignación
        /// </summary>
        Available = 1,

        /// <summary>
        /// En ruta transportando envíos
        /// </summary>
        InTransit = 2,

        /// <summary>
        /// En mantenimiento programado o reparación
        /// </summary>
        Maintenance = 3,

        /// <summary>
        /// Fuera de servicio temporalmente
        /// </summary>
        OutOfService = 4
    }

    /// <summary>
    /// Tipo de combustible del vehículo
    /// </summary>
    public enum FuelType
    {
        /// <summary>
        /// Gasolina
        /// </summary>
        Gasoline = 1,

        /// <summary>
        /// Diesel
        /// </summary>
        Diesel = 2,

        /// <summary>
        /// Gas Natural Vehicular
        /// </summary>
        GNV = 3,

        /// <summary>
        /// Eléctrico
        /// </summary>
        Electric = 4,

        /// <summary>
        /// Híbrido
        /// </summary>
        Hybrid = 5
    }
}