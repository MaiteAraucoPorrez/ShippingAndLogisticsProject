namespace ShippingAndLogisticsManagement.Core.Enum
{
    /// <summary>
    /// Tipo de vehículo según su categoría y uso
    /// </summary>
    public enum VehicleType
    {
        /// <summary>
        /// Motocicleta - Para entregas rápidas urbanas
        /// </summary>
        Motorcycle = 1,

        /// <summary>
        /// Camioneta - Para entregas medianas
        /// </summary>
        Van = 2,

        /// <summary>
        /// Pickup - Para cargas medianas/pesadas
        /// </summary>
        Pickup = 3,

        /// <summary>
        /// Camión - Para grandes volúmenes
        /// </summary>
        Truck = 4
    }
}
