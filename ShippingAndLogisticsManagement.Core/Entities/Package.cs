using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Core.Entities
{
    /// <summary>
    /// Representa un paquete en el sistema de logística
    /// </summary>
    /// <remarks>
    /// Esta entidad almacena la información de paquetes individuales
    /// que forman parte de un envío. Incluye descripción, peso y precio.
    /// </remarks>
    public partial class Package: BaseEntity
    {
        //public int Id { get; set; }

        /// <summary>
        /// Descripción del contenido del paquete
        /// </summary>
        /// <example>Laptop Dell XPS 15</example>
        public string Description { get; set; } = null!;
        /// <summary>
        /// Peso del paquete en kilogramos
        /// </summary>
        /// <example>2.5</example>
        public double Weight { get; set; }
        /// <summary>
        /// Identificador del envío al que pertenece
        /// </summary>
        /// <example>5</example>
        public int ShipmentId { get; set; }
        /// <summary>
        /// Valor declarado del paquete en la moneda local
        /// </summary>
        /// <example>1500.00</example>
        public decimal Price { get; set; }

        public Shipment Shipment { get; set; } = null!;
    }
}