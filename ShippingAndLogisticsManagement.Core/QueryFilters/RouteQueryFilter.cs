using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ShippingAndLogisticsManagement.Core.QueryFilters
{
    /// <summary>
    /// Represents query filters for routes with pagination support
    /// </summary>
    /// <remarks>
    /// Allows filtering routes by origin, destination, distance range, cost range, and active status.
    /// All filters are optional.
    /// </remarks>
    public class RouteQueryFilter : PaginationQueryFilter
    {
        /// <summary>
        /// Route origin city or location
        /// </summary>
        /// <example>Cochabamba</example>
        [SwaggerSchema("Filtrar por ciudad de origen (búsqueda parcial)", Nullable = true)]
        [MaxLength(100, ErrorMessage = "El origen no puede exceder 100 caracteres")]
        public string? Origin { get; set; }

        /// <summary>
        /// Route destination city or location
        /// </summary>
        /// <example>Santa Cruz</example>
        [SwaggerSchema("Filtrar por ciudad de destino (búsqueda parcial)", Nullable = true)]
        [MaxLength(100, ErrorMessage = "El destino no puede exceder 100 caracteres")]
        public string? Destination { get; set; }

        private double? _minDistance;
        /// <summary>
        /// Minimum route distance in kilometers
        /// </summary>
        /// <example>100.0</example>
        [SwaggerSchema("Distancia mínima en km", Nullable = true)]
        [Range(0, double.MaxValue, ErrorMessage = "La distancia mínima no puede ser negativa")]
        public double? MinDistance
        {
            get => _minDistance;
            set
            {
                if (value.HasValue && value.Value < 0)
                    throw new ArgumentException("La distancia mínima no puede ser negativa");
                _minDistance = value;
            }
        }

        private double? _maxDistance;
        /// <summary>
        /// Maximum route distance in kilometers
        /// </summary>
        /// <example>1000.0</example>
        [SwaggerSchema("Distancia máxima en km", Nullable = true)]
        [Range(0, double.MaxValue, ErrorMessage = "La distancia máxima no puede ser negativa")]
        public double? MaxDistance
        {
            get => _maxDistance;
            set
            {
                if (value.HasValue && MinDistance.HasValue && value.Value < MinDistance.Value)
                    throw new ArgumentException("La distancia máxima debe ser mayor o igual a la distancia mínima");
                _maxDistance = value;
            }
        }

        private double? _minCost;
        /// <summary>
        /// Minimum base cost
        /// </summary>
        /// <example>10.0</example>
        [SwaggerSchema("Costo base mínimo", Nullable = true)]
        [Range(0, double.MaxValue, ErrorMessage = "El costo mínimo no puede ser negativo")]
        public double? MinCost
        {
            get => _minCost;
            set
            {
                if (value.HasValue && value.Value < 0)
                    throw new ArgumentException("El costo mínimo no puede ser negativo");
                _minCost = value;
            }
        }

        private double? _maxCost;
        /// <summary>
        /// Maximum base cost
        /// </summary>
        /// <example>500.0</example>
        [SwaggerSchema("Costo base máximo", Nullable = true)]
        public double? MaxCost
        {
            get => _maxCost;
            set
            {
                if (value.HasValue && MinCost.HasValue && value.Value < MinCost.Value)
                    throw new ArgumentException("El costo máximo debe ser mayor o igual al costo mínimo");
                _maxCost = value;
            }
        }

        /// <summary>
        /// Filter by active/inactive routes
        /// </summary>
        /// <example>true</example>
        [SwaggerSchema("Filtrar por rutas activas/inactivas", Nullable = true)]
        public bool? IsActive { get; set; }

        /// <summary>
        /// Validates the filter object
        /// </summary>
        /// <returns>True if valid, throws exception if invalid</returns>
        public bool Validate()
        {
            if (!string.IsNullOrWhiteSpace(Origin) && Origin.Length > 100)
                throw new ArgumentException("El origen no puede exceder 100 caracteres");

            if (!string.IsNullOrWhiteSpace(Destination) && Destination.Length > 100)
                throw new ArgumentException("El destino no puede exceder 100 caracteres");

            if (MinDistance.HasValue && MinDistance.Value < 0)
                throw new ArgumentException("La distancia mínima no puede ser negativa");

            if (MaxDistance.HasValue && MinDistance.HasValue && MaxDistance.Value < MinDistance.Value)
                throw new ArgumentException("La distancia máxima debe ser mayor o igual a la distancia mínima");

            if (MinCost.HasValue && MinCost.Value < 0)
                throw new ArgumentException("El costo mínimo no puede ser negativo");

            if (MaxCost.HasValue && MinCost.HasValue && MaxCost.Value < MinCost.Value)
                throw new ArgumentException("El costo máximo debe ser mayor o igual al costo mínimo");

            return true;
        }
    }
}
