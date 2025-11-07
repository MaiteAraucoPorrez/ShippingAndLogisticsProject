namespace ShippingAndLogisticsManagement.Core.Entities
{
    /// <summary>
    /// Representa un envío en el sistema de logística
    /// </summary>
    /// <remarks>
    /// Esta entidad gestiona los envíos realizados por clientes.
    /// Relaciona cliente, ruta y múltiples paquetes en un solo envío.
    /// 
    /// Reglas de negocio:
    /// - Cliente debe existir y no tener más de 3 envíos activos
    /// - Ruta debe existir y estar activa
    /// - Estado inicial debe ser "Pending"
    /// - Tracking number debe ser único
    /// - No puede pasar a "Delivered" sin estar primero "In transit"
    /// - Un envío puede contener máximo 50 paquetes
    /// </remarks>
    public partial class Shipment: BaseEntity
    {
        /// <summary>
        /// Identificador único del envío
        /// </summary>
        /// <example>1</example>
        // public int Id { get; set; } // Heredado de BaseEntity

        /// <summary>
        /// Fecha y hora en que se realiza el envío
        /// </summary>
        /// <remarks>
        /// No puede ser una fecha pasada al momento de crear el envío.
        /// Se utiliza para calcular tiempos de entrega estimados.
        /// </remarks>
        /// <example>2025-01-15T10:30:00</example>
        public DateTime ShippingDate { get; set; }

        /// <summary>
        /// Estado actual del envío
        /// </summary>
        /// <remarks>
        /// Valores permitidos: "Pending", "In transit", "Delivered"
        /// Estados permitidos por transición:
        /// - Pending → In transit
        /// - In transit → Delivered
        /// - NO se permite: Pending → Delivered (debe pasar por In transit)
        /// </remarks>
        /// <example>Pending</example>
        public required string State { get; set; }

        /// <summary>
        /// Identificador del cliente que realiza el envío
        /// </summary>
        /// <remarks>
        /// Relación obligatoria con la entidad Customer.
        /// El cliente debe existir y no puede tener más de 3 envíos activos.
        /// </remarks>
        /// <example>5</example>
        public int CustomerId { get; set; }

        /// <summary>
        /// Identificador de la ruta asignada al envío
        /// </summary>
        /// <remarks>
        /// Relación obligatoria con la entidad Route.
        /// La ruta debe existir y estar activa (IsActive = true).
        /// </remarks>
        /// <example>3</example>
        public int RouteId { get; set; }

        /// <summary>
        /// Costo total del envío
        /// </summary>
        /// <remarks>
        /// Debe ser mayor a 0.
        /// Incluye costo base de ruta + costos adicionales por peso/volumen.
        /// </remarks>
        /// <example>150.50</example>
        public double TotalCost { get; set; }

        /// <summary>
        /// Número único de seguimiento del envío
        /// </summary>
        /// <remarks>
        /// Debe ser único en todo el sistema.
        /// Máximo 50 caracteres.
        /// Se utiliza para rastrear el envío por el cliente.
        /// </remarks>
        /// <example>TRACK123456789</example>
        public string TrackingNumber { get; set; } = null!;

        /// <summary>
        /// Navegación hacia el cliente asociado
        /// </summary>
        /// <remarks>
        /// Propiedad de navegación de Entity Framework Core.
        /// Permite acceder a los datos completos del cliente desde el envío.
        /// </remarks>
        public Customer Customer { get; set; } = null!;

        /// <summary>
        /// Colección de paquetes incluidos en este envío
        /// </summary>
        /// <remarks>
        /// Propiedad de navegación de Entity Framework Core.
        /// Un envío puede contener entre 0 y 50 paquetes.
        /// Los paquetes se eliminan en cascada si se elimina el envío.
        /// </remarks>
        public ICollection<Package> Packages { get; set; } = new List<Package>();
    }
}
