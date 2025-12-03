namespace ShippingAndLogisticsManagement.Core.CustomEntities
{
    /// <summary>
    /// Represents pagination metadata for a paged collection, including information about the current page, total
    /// items, and navigation availability.
    /// </summary>
    /// <remarks>Use this class to convey paging details when returning paged results from APIs or data
    /// queries. It provides properties to help clients understand the structure of the paged data and to implement
    /// navigation controls such as 'Next' and 'Previous' page buttons.</remarks>
    public class Pagination
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public Pagination()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Pagination class using the specified paged list data.
        /// </summary>
        /// <remarks>This constructor copies pagination information such as total count, page size,
        /// current page, and navigation flags from the provided PagedList object. The items themselves are not stored
        /// or exposed by the Pagination instance.</remarks>
        /// <param name="lista">The paged list containing the items and pagination metadata to initialize the Pagination instance.</param>
        public Pagination(PagedList<object> lista)
        {
            TotalCount = lista.TotalCount;
            PageSize = lista.PageSize;
            CurrentPage = lista.CurrentPage;
            TotalPages = lista.TotalPages;
            HasNextPage = lista.HasNextPage;
            HasPreviousPage = lista.HasPreviousPage;
        }
    }
}