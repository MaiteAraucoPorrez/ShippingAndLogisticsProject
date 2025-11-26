namespace ShippingAndLogisticsManagement.Core.Entities
{
    /// <summary>
    /// Representa una dirección de recogida o entrega en el sistema
    /// </summary>
    /// <remarks>
    /// Esta entidad almacena direcciones asociadas a clientes.
    /// Permite tener múltiples direcciones por cliente (casa, oficina, etc.)
    /// y soporta tanto direcciones de recogida como de entrega.
    /// 
    /// Reglas de negocio:
    /// - Cada cliente puede tener múltiples direcciones
    /// - Solo una dirección puede ser marcada como predeterminada por tipo
    /// - La calle debe tener entre 5 y 200 caracteres
    /// - Ciudad y departamento son obligatorios
    /// </remarks>
    public class Address : BaseEntity
    {
        /// <summary>
        /// Identificador único de la dirección
        /// </summary>
        /// <example>1</example>
        // public int Id { get; set; } // Heredado de BaseEntity

        /// <summary>
        /// ID del cliente propietario de esta dirección
        /// </summary>
        /// <example>5</example>
        public int CustomerId { get; set; }

        /// <summary>
        /// Dirección completa: calle, número, edificio
        /// </summary>
        /// <remarks>
        /// Debe tener entre 5 y 200 caracteres.
        /// Incluir toda la información necesaria para localizar el lugar.
        /// </remarks>
        /// <example>Av. 6 de Agosto #2170, Edificio Torres del Poeta, Piso 5</example>
        public string Street { get; set; } = null!;

        /// <summary>
        /// Ciudad donde se ubica la dirección
        /// </summary>
        /// <remarks>
        /// Debe tener entre 3 y 100 caracteres.
        /// </remarks>
        /// <example>La Paz</example>
        public string City { get; set; } = null!;

        /// <summary>
        /// Departamento de Bolivia
        /// </summary>
        /// <remarks>
        /// Debe ser uno de los 9 departamentos de Bolivia.
        /// </remarks>
        /// <example>La Paz</example>
        public string Department { get; set; } = null!;

        /// <summary>
        /// Zona o barrio (opcional)
        /// </summary>
        /// <remarks>
        /// Ayuda a especificar mejor la ubicación dentro de la ciudad.
        /// </remarks>
        /// <example>Sopocachi</example>
        public string? Zone { get; set; }

        /// <summary>
        /// Código postal
        /// </summary>
        /// <remarks>
        /// En Bolivia no todos los lugares tienen código postal asignado.
        /// </remarks>
        /// <example>0000</example>
        public string? PostalCode { get; set; }

        /// <summary>
        /// Tipo de dirección: recogida o entrega
        /// </summary>
        /// <example>Delivery</example>
        public AddressType Type { get; set; }

        /// <summary>
        /// Indica si es la dirección predeterminada para este tipo
        /// </summary>
        /// <remarks>
        /// Solo puede haber una dirección predeterminada por tipo (Pickup/Delivery) por cliente.
        /// Al marcar una dirección como predeterminada, las demás del mismo tipo se desmarcan automáticamente.
        /// </remarks>
        /// <example>true</example>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Referencia o punto de ubicación adicional
        /// </summary>
        /// <remarks>
        /// Información extra que ayuda a encontrar la dirección.
        /// </remarks>
        /// <example>Cerca del Monoblock Central, puerta verde con rejas negras</example>
        public string? Reference { get; set; }

        /// <summary>
        /// Nombre descriptivo de la dirección (opcional)
        /// </summary>
        /// <remarks>
        /// Permite al cliente identificar fácilmente la dirección.
        /// </remarks>
        /// <example>Mi Casa, Oficina, Casa de Mamá</example>
        public string? Alias { get; set; }

        /// <summary>
        /// Nombre de contacto en esta dirección
        /// </summary>
        /// <remarks>
        /// Persona que recibirá o entregará el paquete.
        /// </remarks>
        /// <example>Juan Pérez</example>
        public string? ContactName { get; set; }

        /// <summary>
        /// Teléfono de contacto en esta dirección
        /// </summary>
        /// <remarks>
        /// Puede ser diferente al teléfono del cliente.
        /// </remarks>
        /// <example>71234567</example>
        public string? ContactPhone { get; set; }

        /// <summary>
        /// Latitud GPS (opcional)
        /// </summary>
        /// <remarks>
        /// Útil para integración con mapas y optimización de rutas.
        /// </remarks>
        /// <example>-16.5000</example>
        public double? Latitude { get; set; }

        /// <summary>
        /// Longitud GPS (opcional)
        /// </summary>
        /// <example>-68.1500</example>
        public double? Longitude { get; set; }

        /// <summary>
        /// Indica si la dirección está activa
        /// </summary>
        /// <remarks>
        /// Permite desactivar direcciones sin eliminarlas del sistema.
        /// </remarks>
        /// <example>true</example>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Fecha de creación de la dirección
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Navegación al cliente propietario
        /// </summary>
        /// <remarks>
        /// Propiedad de navegación de Entity Framework Core.
        /// </remarks>
        public Customer Customer { get; set; } = null!;
    }

    /// <summary>
    /// Tipo de dirección en el sistema
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// Dirección de recogida de paquetes
        /// </summary>
        Pickup = 1,

        /// <summary>
        /// Dirección de entrega de paquetes
        /// </summary>
        Delivery = 2
    }
}