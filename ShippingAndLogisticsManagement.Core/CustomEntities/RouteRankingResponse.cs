namespace ShippingAndLogisticsManagement.Core.CustomEntities
{
    /// <summary>
    /// Representa el ranking de una ruta basado en su uso.
    /// Incluye estadísticas de cantidad de envíos, ingresos generados y popularidad.
    /// </summary>
    public class RouteRankingResponse
    {
        /// <summary>
        /// ID de la ruta
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// Ciudad o ubicación de origen
        /// </summary>
        public string Origin { get; set; } = string.Empty;

        /// <summary>
        /// Ciudad o ubicación de destino
        /// </summary>
        public string Destination { get; set; } = string.Empty;

        /// <summary>
        /// Distancia total de la ruta en kilómetros
        /// </summary>
        public double DistanceKm { get; set; }

        /// <summary>
        /// Costo base de la ruta
        /// </summary>
        public double BaseCost { get; set; }

        /// <summary>
        /// Indica si la ruta está activa
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Cantidad total de envíos realizados por esta ruta
        /// </summary>
        public int TotalShipments { get; set; }

        /// <summary>
        /// Cantidad de envíos activos (no entregados) en la ruta
        /// </summary>
        public int ActiveShipments { get; set; }

        /// <summary>
        /// Cantidad de envíos completados (entregados) en la ruta
        /// </summary>
        public int CompletedShipments { get; set; }

        /// <summary>
        /// Ingreso total generado por todos los envíos en esta ruta
        /// </summary>
        public double TotalRevenue { get; set; }

        /// <summary>
        /// Ingreso promedio por envío en esta ruta
        /// </summary>
        public double AverageRevenue { get; set; }

        /// <summary>
        /// Costo por kilómetro calculado (BaseCost / DistanceKm)
        /// </summary>
        public double CostPerKm { get; set; }

        /// <summary>
        /// Posición en el ranking (1 = más utilizada)
        /// </summary>
        public int Rank { get; set; }
    }
}
