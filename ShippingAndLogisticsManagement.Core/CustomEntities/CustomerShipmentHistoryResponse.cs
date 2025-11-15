namespace ShippingAndLogisticsManagement.Core.CustomEntities
{
    /// <summary>
    /// Representa el historial de envíos de un cliente con información detallada.
    /// Incluye datos del envío, ruta, paquetes y estados.
    /// </summary>
    public class CustomerShipmentHistoryResponse
    {
        /// <summary>
        /// ID del envío
        /// </summary>
        public int ShipmentId { get; set; }

        /// <summary>
        /// Número de seguimiento del envío
        /// </summary>
        public string TrackingNumber { get; set; } = string.Empty;

        /// <summary>
        /// Fecha en que se realizó el envío
        /// </summary>
        public DateTime ShippingDate { get; set; }

        /// <summary>
        /// Estado actual del envío (Pending, In transit, Delivered)
        /// </summary>
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// Costo total del envío
        /// </summary>
        public double TotalCost { get; set; }

        /// <summary>
        /// Origen de la ruta del envío
        /// </summary>
        public string RouteOrigin { get; set; } = string.Empty;

        /// <summary>
        /// Destino de la ruta del envío
        /// </summary>
        public string RouteDestination { get; set; } = string.Empty;

        /// <summary>
        /// Distancia de la ruta en kilómetros
        /// </summary>
        public double RouteDistanceKm { get; set; }

        /// <summary>
        /// Cantidad de paquetes en el envío
        /// </summary>
        public int PackageCount { get; set; }

        /// <summary>
        /// Peso total de todos los paquetes
        /// </summary>
        public double TotalWeight { get; set; }

        /// <summary>
        /// Valor total declarado de todos los paquetes
        /// </summary>
        public decimal TotalValue { get; set; }
    }
}
