using ShippingAndLogisticsManagement.Core.Enum;

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
        /// Número de licencia de conducir (único)
        /// </summary>
        /// <example>LIC-2024-001234</example>
        public string LicenseNumber { get; set; } = null!;

        /// <summary>
        /// Teléfono de contacto principal
        /// </summary>
        /// <example>71234567</example>
        public string Phone { get; set; } = null!;

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
        /// Total de entregas completadas
        /// </summary>
        /// <example>250</example>
        public int TotalDeliveries { get; set; }

        /// <summary>
        /// Navegación al vehículo actual
        /// </summary>
        public Vehicle? CurrentVehicle { get; set; }
    }
}