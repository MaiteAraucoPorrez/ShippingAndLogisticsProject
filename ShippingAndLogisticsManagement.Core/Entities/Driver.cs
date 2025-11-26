using ShippingAndLogisticsManagement.Core.Enum;

namespace ShippingAndLogisticsManagement.Core.Entities
{
    /// <summary>
    /// Representa un conductor
    /// </summary>
    public class Driver : BaseEntity
    {
        /// <summary>
        /// Nombre completo
        /// </summary>
        public string FullName { get; set; } = null!;

        /// <summary>
        /// Número de licencia de conducir
        /// </summary>
        public string LicenseNumber { get; set; } = null!;

        /// <summary>
        /// Categoría de licencia
        /// </summary>
        /// <example>Categoría B, C</example>
        public string LicenseCategory { get; set; } = null!;

        /// <summary>
        /// Fecha de vencimiento de licencia
        /// </summary>
        public DateTime LicenseExpiryDate { get; set; }

        /// <summary>
        /// Teléfono de contacto
        /// </summary>
        public string Phone { get; set; } = null!;

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Fecha de contratación
        /// </summary>
        public DateTime HireDate { get; set; }

        /// <summary>
        /// ¿Conductor activo?
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Estado actual
        /// </summary>
        public DriverStatus Status { get; set; } = DriverStatus.Available;

        /// <summary>
        /// Vehículo actualmente asignado
        /// </summary>
        public int? CurrentVehicleId { get; set; }

        public Vehicle? CurrentVehicle { get; set; }
    }
}
