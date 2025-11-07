using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ShippingAndLogisticsManagement.Core.QueryFilters
{
    /// <summary>
    /// Represents query filters for packages with pagination support
    /// </summary>
    /// <remarks>
    /// Allows filtering packages by multiple criteria including shipment ID,
    /// weight range, price range, and description. All filters are optional.
    /// </remarks>
    public class PackageQueryFilter : PaginationQueryFilter
    {
        /// <summary>
        /// Shipment ID to which the package belongs
        /// </summary>
        /// <example>5</example>
        [SwaggerSchema("Filtrar por ID de envio", Nullable = true)]
        public int? ShipmentId { get; set; }

        /// <summary>
        /// Description or part of the package description
        /// </summary>
        /// <example>Laptop</example>
        [SwaggerSchema("Buscar por descripcion del paquete", Nullable = true)]
        [MaxLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
        public string? Description { get; set; }

        private double? _minWeight;
        /// <summary>
        /// Minimum package weight in kilograms
        /// </summary>
        /// <example>1.0</example>
        [SwaggerSchema("Peso minimo en kg", Nullable = true)]
        [Range(0, double.MaxValue, ErrorMessage = "El peso mínimo no puede ser negativo")]
        public double? MinWeight
        {
            get => _minWeight;
            set
            {
                if (value.HasValue && value.Value < 0)
                    throw new ArgumentException("El peso mínimo no puede ser negativo");
                _minWeight = value;
            }
        }

        private double? _maxWeight;
        /// <summary>
        /// Maximum package weight in kilograms
        /// </summary>
        /// <example>10.0</example>
        [SwaggerSchema("Peso máximo en kg", Nullable = true)]
        [Range(0, 100, ErrorMessage = "El peso máximo debe estar entre 0 y 100 kg")]
        public double? MaxWeight
        {
            get => _maxWeight;
            set
            {
                if (value.HasValue && MinWeight.HasValue && value.Value < MinWeight.Value)
                    throw new ArgumentException("El peso máximo debe ser mayor o igual al peso mínimo");
                _maxWeight = value;
            }
        }

        private decimal? _minPrice;
        /// <summary>
        /// Minimum package price
        /// </summary>
        /// <example>100.00</example>
        [SwaggerSchema("Precio mínimo del paquete", Nullable = true)]
        [Range(0, double.MaxValue, ErrorMessage = "El precio mínimo no puede ser negativo")]
        public decimal? MinPrice
        {
            get => _minPrice;
            set
            {
                if (value.HasValue && value.Value < 0)
                    throw new ArgumentException("El precio mínimo no puede ser negativo");
                _minPrice = value;
            }
        }

        private decimal? _maxPrice;
        /// <summary>
        /// Maximum package price
        /// </summary>
        /// <example>5000.00</example>
        [SwaggerSchema("Precio máximo del paquete", Nullable = true)]
        public decimal? MaxPrice
        {
            get => _maxPrice;
            set
            {
                if (value.HasValue && MinPrice.HasValue && value.Value < MinPrice.Value)
                    throw new ArgumentException("El precio máximo debe ser mayor o igual al precio mínimo");
                _maxPrice = value;
            }
        }

        /// <summary>
        /// Validates the filter object
        /// </summary>
        /// <returns>True if valid, throws exception if invalid</returns>
        public bool Validate()
        {
            if (MinWeight.HasValue && MinWeight.Value < 0)
                throw new ArgumentException("El peso mínimo no puede ser negativo");

            if (MaxWeight.HasValue && MinWeight.HasValue && MaxWeight.Value < MinWeight.Value)
                throw new ArgumentException("El peso máximo debe ser mayor o igual al peso mínimo");

            if (MinPrice.HasValue && MinPrice.Value < 0)
                throw new ArgumentException("El precio mínimo no puede ser negativo");

            if (MaxPrice.HasValue && MinPrice.HasValue && MaxPrice.Value < MinPrice.Value)
                throw new ArgumentException("El precio máximo debe ser mayor o igual al precio mínimo");

            return true;
        }
    }
}