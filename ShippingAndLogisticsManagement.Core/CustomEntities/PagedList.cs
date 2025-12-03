namespace ShippingAndLogisticsManagement.Core.CustomEntities
{
    /// <summary>
    /// Represents a paged subset of a collection of items, including pagination metadata such as the current page,
    /// total pages, and total item count.
    /// </summary>
    /// <remarks>Use this class to manage and navigate large collections by dividing them into discrete pages.
    /// The pagination properties provide information for UI navigation and data retrieval scenarios. The class inherits
    /// from <see cref="List{T}"/>, allowing standard list operations on the current page's items.</remarks>
    /// <typeparam name="T">The type of elements contained in the paged list.</typeparam>
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int? NextPageNumber => HasNextPage ? CurrentPage + 1 : null;
        public int? PreviousPageNumber => HasPreviousPage ? CurrentPage - 1 : null;

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }

        public static PagedList<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
