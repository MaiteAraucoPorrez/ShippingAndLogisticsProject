using ShippingAndLogisticsManagement.Core.Enum;

namespace ShippingAndLogisticsManagement.Core.Entities
{
    /// <summary>
    /// Representa el paso de un envío por un almacén (tracking)
    /// </summary>
    /// <remarks>
    /// Esta tabla intermedia registra el historial de ubicaciones de cada envío,
    /// permitiendo rastrear su trayectoria por la red de almacenes.
    /// 
    /// Flujo típico:
    /// 1. Received: Envío llega al almacén
    /// 2. InStorage: En almacenamiento temporal
    /// 3. Processing: En proceso de clasificación
    /// 4. Dispatched: Sale hacia otro almacén o destino final
    /// </remarks>
    public class ShipmentWarehouse : BaseEntity
    {
        /// <summary>
        /// Identificador único del registro
        /// </summary>
        /// <example>1</example>
        // public int Id { get; set; } // Heredado de BaseEntity

        /// <summary>
        /// ID del envío
        /// </summary>
        /// <example>5</example>
        public int ShipmentId { get; set; }

        /// <summary>
        /// ID del almacén
        /// </summary>
        /// <example>2</example>
        public int WarehouseId { get; set; }

        /// <summary>
        /// Fecha y hora de entrada al almacén
        /// </summary>
        /// <example>2025-01-15T10:30:00</example>
        public DateTime EntryDate { get; set; }

        /// <summary>
        /// Fecha y hora de salida del almacén
        /// </summary>
        /// <remarks>
        /// Null mientras el envío permanezca en el almacén.
        /// </remarks>
        /// <example>2025-01-16T14:00:00</example>
        public DateTime? ExitDate { get; set; }

        /// <summary>
        /// Estado del envío en este almacén
        /// </summary>
        /// <example>InStorage</example>
        public WarehouseShipmentStatus Status { get; set; }

        /// <summary>
        /// Usuario que registró la entrada
        /// </summary>
        /// <example>jperez</example>
        public string? ReceivedBy { get; set; }

        /// <summary>
        /// Usuario que registró la salida
        /// </summary>
        /// <example>mrodriguez</example>
        public string? DispatchedBy { get; set; }

        /// <summary>
        /// Observaciones sobre este paso por el almacén
        /// </summary>
        /// <example>Paquete con etiqueta dañada, se reemplazó</example>
        public string? Notes { get; set; }

        /// <summary>
        /// Ubicación específica dentro del almacén
        /// </summary>
        /// <example>Estante A-12, Nivel 3</example>
        public string? StorageLocation { get; set; }

        /// <summary>
        /// Navegación al envío
        /// </summary>
        public Shipment Shipment { get; set; } = null!;

        /// <summary>
        /// Navegación al almacén
        /// </summary>
        public Warehouse Warehouse { get; set; } = null!;
    }
}