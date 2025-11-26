namespace ShippingAndLogisticsManagement.Core.CustomEntities
{
    /// <summary>
    /// Estadísticas de operación de un almacén
    /// </summary>
    public class WarehouseStatisticsResponse
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string City { get; set; } = null!;

        /// <summary>
        /// Total de envíos que han pasado por el almacén
        /// </summary>
        public int TotalShipments { get; set; }

        /// <summary>
        /// Envíos actualmente en el almacén
        /// </summary>
        public int CurrentShipments { get; set; }

        /// <summary>
        /// Envíos despachados del almacén
        /// </summary>
        public int DispatchedShipments { get; set; }

        /// <summary>
        /// Porcentaje de ocupación actual
        /// </summary>
        public double OccupancyPercentage { get; set; }

        /// <summary>
        /// Capacidad disponible en m3
        /// </summary>
        public double AvailableCapacity { get; set; }

        /// <summary>
        /// Tiempo promedio de permanencia (en horas)
        /// </summary>
        public double? AverageStayTimeHours { get; set; }
    }
}