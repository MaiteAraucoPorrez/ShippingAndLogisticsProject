namespace ShippingAndLogisticsManagement.Core.Enum
{
    /// <summary>
    /// Estado operativo del vehículo
    /// </summary>
    public enum VehicleStatus
    {
        /// <summary>
        /// Disponible para asignación
        /// </summary>
        Available = 1,

        /// <summary>
        /// En ruta transportando envíos
        /// </summary>
        InTransit = 2,

        /// <summary>
        /// En mantenimiento programado o reparación
        /// </summary>
        Maintenance = 3,

        /// <summary>
        /// Fuera de servicio temporalmente
        /// </summary>
        OutOfService = 4
    }
}
