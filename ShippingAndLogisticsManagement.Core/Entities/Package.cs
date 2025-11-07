using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Core.Entities
{
    /// <summary>
    /// Representa un paquete en el sistema de logística
    /// </summary>
    /// <remarks>
    /// Esta entidad almacena la información de paquetes individuales
    /// que forman parte de un envío. Incluye descripción, peso, precio y 
    /// asociación con el envío correspondiente. Un paquete siempre debe 
    /// estar asociado a un envío existente y válido.
    /// 
    /// Reglas de negocio:
    /// - Peso: Entre 0.1 kg y 100 kg
    /// - Precio: Mayor a 0
    /// - Descripción: Mínimo 3 caracteres, máximo 200
    /// - Límite: 50 paquetes por envío
    /// </remarks>
    public partial class Package: BaseEntity
    {
        /// <summary>
        /// Identificador único del paquete
        /// </summary>
        /// <example>1</example>
        // public int Id { get; set; } // Heredado de BaseEntity

        /// <summary>
        /// Descripción detallada del contenido del paquete
        /// </summary>
        /// <remarks>
        /// Debe tener entre 3 y 200 caracteres.
        /// Se recomienda incluir marca, modelo y características principales.
        /// </remarks>
        /// <example>Laptop Dell XPS 15 - Intel i7, 16GB RAM, 512GB SSD</example>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Peso del paquete en kilogramos
        /// </summary>
        /// <remarks>
        /// Debe estar entre 0.1 kg y 100 kg.
        /// El peso es utilizado para cálculos de costo de envío y capacidad de transporte.
        /// </remarks>
        /// <example>2.5</example>
        public double Weight { get; set; }

        /// <summary>
        /// Identificador del envío al que pertenece el paquete
        /// </summary>
        /// <remarks>
        /// Relación obligatoria con la entidad Shipment.
        /// El envío debe existir y no estar en estado "Delivered" para agregar paquetes.
        /// </remarks>
        /// <example>5</example>
        public int ShipmentId { get; set; }

        /// <summary>
        /// Valor declarado del paquete en la moneda local
        /// </summary>
        /// <remarks>
        /// Representa el valor asegurado del contenido del paquete.
        /// Debe ser mayor a 0. Se utiliza para cálculos de seguro y cobertura.
        /// </remarks>
        /// <example>1500.00</example>
        public decimal Price { get; set; }

        /// <summary>
        /// Navegación hacia el envío asociado
        /// </summary>
        /// <remarks>
        /// Propiedad de navegación de Entity Framework Core.
        /// Permite acceder a los datos completos del envío desde el paquete.
        /// </remarks>
        public Shipment Shipment { get; set; } = null!;
    }
}