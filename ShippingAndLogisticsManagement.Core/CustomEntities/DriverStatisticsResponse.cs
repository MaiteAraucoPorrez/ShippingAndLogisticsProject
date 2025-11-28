namespace ShippingAndLogisticsManagement.Core.CustomEntities
{
    /// <summary>
    /// Estadísticas de desempeño de un conductor
    /// </summary>
    public class DriverStatisticsResponse
    {
        public int DriverId { get; set; }
        public string FullName { get; set; } = null!;
        public string LicenseNumber { get; set; } = null!;

        /// <summary>
        /// Total de entregas realizadas
        /// </summary>
        public int TotalDeliveries { get; set; }

        /// <summary>
        /// Entregas completadas a tiempo
        /// </summary>
        public int OnTimeDeliveries { get; set; }

        /// <summary>
        /// Entregas con retraso
        /// </summary>
        public int LateDeliveries { get; set; }

        /// <summary>
        /// Porcentaje de entregas a tiempo
        /// </summary>
        public double OnTimePercentage { get; set; }

        /// <summary>
        /// Calificación promedio
        /// </summary>
        public double? AverageRating { get; set; }

        /// <summary>
        /// Años de experiencia
        /// </summary>
        public int YearsOfExperience { get; set; }

        /// <summary>
        /// Días hasta vencimiento de licencia
        /// </summary>
        public int DaysUntilLicenseExpiry { get; set; }

        /// <summary>
        /// ¿Licencia próxima a vencer? (menos de 30 días)
        /// </summary>
        public bool LicenseExpiringSoon { get; set; }
    }
}