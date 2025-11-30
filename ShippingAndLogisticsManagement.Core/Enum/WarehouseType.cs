namespace ShippingAndLogisticsManagement.Core.Enum
{
    /// <summary>
    /// Tipo de almacén según su nivel en la red de distribución
    /// </summary>
    public enum WarehouseType
    {
        /// <summary>
        /// Almacén principal - Hub nacional
        /// </summary>
        Central = 1,

        /// <summary>
        /// Almacén regional - Hub departamental
        /// </summary>
        Regional = 2,

        /// <summary>
        /// Almacén local - Distribución final
        /// </summary>
        Local = 3
    }
}
