using Swashbuckle.AspNetCore.Annotations;

namespace ShippingAndLogisticsManagement.Core.QueryFilters
{
    public abstract class PaginationQueryFilter
    {
        /// <summary>
        /// Gets or sets the number of items to display per page.
        /// </summary>
        [SwaggerSchema("Cantidad de registros por pagina")]
        public int PageSize { get; set; }


        /// <summary>
        /// Gets or sets the current page number in a paginated list.
        /// </summary>
        [SwaggerSchema("Numero de pagina a mostrar")]
        public int PageNumber { get; set; }
    }
}
