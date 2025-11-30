namespace ShippingAndLogisticsManagement.Core.Enum
{
    /// <summary>
    /// Estado operativo del conductor
    /// </summary>
    public enum DriverStatus
    {
        /// <summary>
        /// Disponible para asignación
        /// </summary>
        Available = 1,

        /// <summary>
        /// En ruta realizando entregas
        /// </summary>
        OnRoute = 2,

        /// <summary>
        /// Fuera de servicio (descanso, vacaciones, etc.)
        /// </summary>
        OffDuty = 3,

        /// <summary>
        /// En licencia médica
        /// </summary>
        OnLeave = 4
    }
}
