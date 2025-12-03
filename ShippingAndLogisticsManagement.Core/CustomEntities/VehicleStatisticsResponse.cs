namespace ShippingAndLogisticsManagement.Core.CustomEntities
{
    /// <summary>
    /// Estadísticas de operación de un vehículo
    /// </summary>
    public class VehicleStatisticsResponse
    {
        public int VehicleId { get; set; }
        public string PlateNumber { get; set; } = null!;

        /// <summary>
        /// Total de envíos transportados
        /// </summary>
        public int TotalShipments { get; set; }

        /// <summary>
        /// Envíos actualmente en tránsito
        /// </summary>
        public int CurrentShipments { get; set; }

        /// <summary>
        /// Envíos completados
        /// </summary>
        public int CompletedShipments { get; set; }

        /// <summary>
        /// Porcentaje de ocupación de peso
        /// </summary>
        public double WeightOccupancyPercentage { get; set; }

        /// <summary>
        /// Porcentaje de ocupación de volumen
        /// </summary>
        public double VolumeOccupancyPercentage { get; set; }

        /// <summary>
        /// Peso disponible en kg
        /// </summary>
        public double AvailableWeightKg { get; set; }

        /// <summary>
        /// Volumen disponible en m³
        /// </summary>
        public double AvailableVolumeM3 { get; set; }
    }
}