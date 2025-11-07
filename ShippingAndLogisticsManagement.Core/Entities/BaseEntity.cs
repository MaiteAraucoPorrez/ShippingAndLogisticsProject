namespace ShippingAndLogisticsManagement.Core.Entities
{
    /// <summary>
    /// Clase base para todas las entidades del sistema
    /// </summary>
    /// <remarks>
    /// Proporciona el identificador único común a todas las entidades.
    /// Todas las entidades del dominio heredan de esta clase.
    /// </remarks>
    public class BaseEntity
    {
        /// <summary>
        /// Identificador único de la entidad
        /// </summary>
        /// <remarks>
        /// Auto-generado por la base de datos.
        /// Clave primaria de todas las entidades.
        /// </remarks>
        /// <example>1</example>
        public int Id { get; set; }
    }
}
