namespace AMZN.DTOs.Common
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int Page {  get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public int TotalPages
        {
            get
            {
                if (PageSize <= 0)
                    return 0;

                var pages = TotalItems / PageSize;

                if (TotalItems % PageSize > 0)
                    pages++;

                return pages;
            }
        }

        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;

    }
}
