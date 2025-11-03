namespace ShippingAndLogisticsManagement.Core.CustomEntities
{
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