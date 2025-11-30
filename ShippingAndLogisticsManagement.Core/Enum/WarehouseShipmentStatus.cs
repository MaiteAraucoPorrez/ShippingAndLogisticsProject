namespace ShippingAndLogisticsManagement.Core.Enum
{
    /// <summary>
    /// Estado de un envío dentro de un almacén
    /// </summary>
    public enum WarehouseShipmentStatus
    {
        /// <summary>
        /// Recién recibido en el almacén
        /// </summary>
        Received = 1,

        /// <summary>
        /// En almacenamiento temporal
        /// </summary>
        InStorage = 2,

        /// <summary>
        /// En proceso de clasificación/preparación
        /// </summary>
        Processing = 3,

        /// <summary>
        /// Despachado del almacén
        /// </summary>
        Dispatched = 4
    }
}
