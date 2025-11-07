namespace ShippingAndLogisticsManagement.Core.Entities
{
    /// <summary>
    /// Representa una ruta de envío en el sistema
    /// </summary>
    /// <remarks>
    /// Esta entidad define las rutas disponibles entre ciudades.
    /// Incluye información de origen, destino, distancia y costos.
    /// 
    /// Reglas de negocio:
    /// - Origen y destino deben ser diferentes
    /// - Distancia debe ser mayor a 0 km
    /// - Costo base debe ser mayor a 0
    /// - Solo rutas activas pueden recibir nuevos envíos
    /// </remarks>
    public partial class Route: BaseEntity
    {
        /// <summary>
        /// Identificador único de la ruta
        /// </summary>
        /// <example>1</example>
        // public int Id { get; set; } // Heredado de BaseEntity

        /// <summary>
        /// Ciudad o ubicación de origen
        /// </summary>
        /// <remarks>
        /// Debe tener entre 3 y 100 caracteres.
        /// </remarks>
        /// <example>Cochabamba</example>
        public string Origin { get; set; } = null!;

        /// <summary>
        /// Ciudad o ubicación de destino
        /// </summary>
        /// <remarks>
        /// Debe tener entre 3 y 100 caracteres.
        /// Debe ser diferente del origen.
        /// </remarks>
        /// <example>Santa Cruz</example>
        public string Destination { get; set; } = null!;

        /// <summary>
        /// Distancia total de la ruta en kilómetros
        /// </summary>
        /// <remarks>
        /// Debe ser mayor a 0.
        /// Se utiliza para cálculos de tiempo estimado de entrega.
        /// </remarks>
        /// <example>502.5</example>
        public double DistanceKm { get; set; }

        /// <summary>
        /// Costo base del envío por esta ruta
        /// </summary>
        /// <remarks>
        /// Debe ser mayor a 0.
        /// Es el costo mínimo sin considerar peso o volumen del paquete.
        /// </remarks>
        /// <example>50.00</example>
        public double BaseCost { get; set; }

        /// <summary>
        /// Indica si la ruta está activa y disponible para nuevos envíos
        /// </summary>
        /// <remarks>
        /// Por defecto es true.
        /// Las rutas inactivas no pueden recibir nuevos envíos pero
        /// mantienen los envíos existentes.
        /// </remarks>
        /// <example>true</example>
        public bool IsActive { get; set; } = true;
    }
}
