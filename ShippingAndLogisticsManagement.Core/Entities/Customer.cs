namespace ShippingAndLogisticsManagement.Core.Entities
{
    /// <summary>
    /// Representa un cliente en el sistema de logística
    /// </summary>
    /// <remarks>
    /// Esta entidad almacena la información de clientes que realizan envíos.
    /// Un cliente puede tener múltiples envíos asociados.
    /// 
    /// Reglas de negocio:
    /// - Email debe ser único y válido
    /// - Teléfono debe tener entre 7 y 20 caracteres
    /// - Nombre es obligatorio (3-100 caracteres)
    /// - No puede tener más de 3 envíos activos simultáneos
    /// </remarks>
    public partial class Customer: BaseEntity
    {
        /// <summary>
        /// Identificador único del cliente
        /// </summary>
        /// <example>1</example>
        // public int Id { get; set; } // Heredado de BaseEntity

        /// <summary>
        /// Nombre completo del cliente
        /// </summary>
        /// <remarks>
        /// Debe tener entre 3 y 100 caracteres.
        /// Puede incluir nombres y apellidos completos.
        /// </remarks>
        /// <example>Juan Carlos Pérez González</example>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Correo electrónico del cliente
        /// </summary>
        /// <remarks>
        /// Debe ser único en el sistema y tener formato válido.
        /// Se utiliza para notificaciones de seguimiento de envíos.
        /// </remarks>
        /// <example>juan.perez@email.com</example>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Número de teléfono del cliente
        /// </summary>
        /// <remarks>
        /// Debe tener entre 7 y 20 caracteres.
        /// Puede incluir código de país y área.
        /// </remarks>
        /// <example>+591 12345678</example>
        public string Phone { get; set; } = null!;

        /// <summary>
        /// Colección de envíos realizados por el cliente
        /// </summary>
        /// <remarks>
        /// Propiedad de navegación de Entity Framework Core.
        /// Permite acceder a todos los envíos del cliente.
        /// El cliente puede tener máximo 3 envíos activos simultáneos.
        /// </remarks>
        public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
}
