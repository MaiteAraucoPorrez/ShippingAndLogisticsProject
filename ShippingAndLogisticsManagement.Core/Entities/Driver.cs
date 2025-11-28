namespace ShippingAndLogisticsManagement.Core.Entities
{
    /// <summary>
    /// Representa un conductor de la flota logística
    /// </summary>
    /// <remarks>
    /// Los conductores son responsables de transportar los envíos
    /// desde los almacenes hasta los destinos. Cada conductor puede
    /// ser asignado a un vehículo específico.
    /// 
    /// Reglas de negocio:
    /// - Número de licencia único
    /// - Licencia debe estar vigente (no vencida)
    /// - Solo conductores activos pueden ser asignados
    /// - Un conductor puede manejar solo un vehículo a la vez
    /// </remarks>
    public class Driver : BaseEntity
    {
        /// <summary>
        /// Identificador único del conductor
        /// </summary>
        /// <example>1</example>
        // public int Id { get; set; } // Heredado de BaseEntity

        /// <summary>
        /// Nombre completo del conductor
        /// </summary>
        /// <example>Juan Carlos Pérez González</example>
        public string FullName { get; set; } = null!;

        /// <summary>
        /// Documento de identidad (CI)
        /// </summary>
        /// <example>1234567 LP</example>
        public string IdentityDocument { get; set; } = null!;

        /// <summary>
        /// Número de licencia de conducir (único)
        /// </summary>
        /// <example>LIC-2024-001234</example>
        public string LicenseNumber { get; set; } = null!;

        /// <summary>
        /// Categoría de licencia
        /// </summary>
        /// <remarks>
        /// Categorías comunes en Bolivia:
        /// - Categoría A: Motocicletas
        /// - Categoría B: Vehículos ligeros
        /// - Categoría C: Vehículos pesados
        /// </remarks>
        /// <example>Categoría C</example>
        public string LicenseCategory { get; set; } = null!;

        /// <summary>
        /// Fecha de emisión de la licencia
        /// </summary>
        /// <example>2020-01-15</example>
        public DateTime LicenseIssueDate { get; set; }

        /// <summary>
        /// Fecha de vencimiento de la licencia
        /// </summary>
        /// <example>2025-01-15</example>
        public DateTime LicenseExpiryDate { get; set; }

        /// <summary>
        /// Teléfono de contacto principal
        /// </summary>
        /// <example>71234567</example>
        public string Phone { get; set; } = null!;

        /// <summary>
        /// Teléfono alternativo (opcional)
        /// </summary>
        /// <example>2-2234567</example>
        public string? AlternativePhone { get; set; }

        /// <summary>
        /// Email del conductor
        /// </summary>
        /// <example>juan.perez@empresa.com</example>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Dirección de residencia
        /// </summary>
        /// <example>Av. 6 de Agosto #1234, Zona Sopocachi</example>
        public string? Address { get; set; }

        /// <summary>
        /// Ciudad de residencia
        /// </summary>
        /// <example>La Paz</example>
        public string? City { get; set; }

        /// <summary>
        /// Fecha de nacimiento
        /// </summary>
        /// <example>1985-05-20</example>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Fecha de contratación
        /// </summary>
        /// <example>2020-03-01</example>
        public DateTime HireDate { get; set; }

        /// <summary>
        /// Fecha de fin de contrato (opcional)
        /// </summary>
        /// <example>2025-03-01</example>
        public DateTime? ContractEndDate { get; set; }

        /// <summary>
        /// Años de experiencia conduciendo
        /// </summary>
        /// <example>10</example>
        public int YearsOfExperience { get; set; }

        /// <summary>
        /// Indica si el conductor está activo
        /// </summary>
        /// <example>true</example>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Estado actual del conductor
        /// </summary>
        /// <example>Available</example>
        public DriverStatus Status { get; set; } = DriverStatus.Available;

        /// <summary>
        /// ID del vehículo actualmente asignado
        /// </summary>
        /// <remarks>
        /// Null si el conductor no tiene vehículo asignado.
        /// </remarks>
        /// <example>3</example>
        public int? CurrentVehicleId { get; set; }

        /// <summary>
        /// Calificación promedio del conductor (1-5)
        /// </summary>
        /// <example>4.5</example>
        public double? AverageRating { get; set; }

        /// <summary>
        /// Total de entregas completadas
        /// </summary>
        /// <example>250</example>
        public int TotalDeliveries { get; set; }

        /// <summary>
        /// Nombre del contacto de emergencia
        /// </summary>
        /// <example>María González</example>
        public string? EmergencyContactName { get; set; }

        /// <summary>
        /// Teléfono del contacto de emergencia
        /// </summary>
        /// <example>77654321</example>
        public string? EmergencyContactPhone { get; set; }

        /// <summary>
        /// Tipo de sangre (para emergencias médicas)
        /// </summary>
        /// <example>O+</example>
        public string? BloodType { get; set; }

        /// <summary>
        /// Fecha de registro en el sistema
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Notas adicionales sobre el conductor
        /// </summary>
        /// <example>Especializado en rutas de montaña</example>
        public string? Notes { get; set; }

        /// <summary>
        /// Navegación al vehículo actual
        /// </summary>
        public Vehicle? CurrentVehicle { get; set; }
    }

    /// <summary>
    /// Estado operativo del conductor
    /// </summary>
    public enum DriverStatus
    {
        /// <summary>
        /// Disponible para asignación
        /// </summary>
        Available = 1,

        /// <summary>
        /// En ruta realizando entregas
        /// </summary>
        OnRoute = 2,

        /// <summary>
        /// Fuera de servicio (descanso, vacaciones, etc.)
        /// </summary>
        OffDuty = 3,

        /// <summary>
        /// En licencia médica
        /// </summary>
        OnLeave = 4
    }
}