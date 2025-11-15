using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ShippingAndLogisticsManagement.Core.QueryFilters
{
    /// <summary>
    /// Represents query filters for customers with pagination support
    /// </summary>
    /// <remarks>
    /// Allows filtering customers by name, email, phone, and active shipments status.
    /// All filters are optional.
    /// </remarks>
    public class CustomerQueryFilter : PaginationQueryFilter
    {
        /// <summary>
        /// Customer name or partial name to search
        /// </summary>
        /// <example>Juan</example>
        [SwaggerSchema("Buscar por nombre del cliente (búsqueda parcial)", Nullable = true)]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string? Name { get; set; }

        /// <summary>
        /// Customer email or partial email to search
        /// </summary>
        /// <example>juan@email.com</example>
        [SwaggerSchema("Buscar por email del cliente (búsqueda parcial)", Nullable = true)]
        [MaxLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
        public string? Email { get; set; }

        /// <summary>
        /// Customer phone number or partial phone to search
        /// </summary>
        /// <example>12345678</example>
        [SwaggerSchema("Buscar por teléfono del cliente (búsqueda parcial)", Nullable = true)]
        [MaxLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string? Phone { get; set; }

        /// <summary>
        /// Filter by customers with active shipments
        /// </summary>
        /// <example>true</example>
        [SwaggerSchema("Filtrar solo clientes con envíos activos", Nullable = true)]
        public bool? HasActiveShipments { get; set; }

        /// <summary>
        /// Validates the filter object
        /// </summary>
        /// <returns>True if valid, throws exception if invalid</returns>
        public bool Validate()
        {
            if (!string.IsNullOrWhiteSpace(Name) && Name.Length > 100)
                throw new ArgumentException("El nombre no puede exceder 100 caracteres");

            if (!string.IsNullOrWhiteSpace(Email) && Email.Length > 100)
                throw new ArgumentException("El email no puede exceder 100 caracteres");

            if (!string.IsNullOrWhiteSpace(Phone) && Phone.Length > 20)
                throw new ArgumentException("El teléfono no puede exceder 20 caracteres");

            return true;
        }
    }
}
