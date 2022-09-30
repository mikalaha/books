using BooksApi.Data;

namespace BooksApi.Extensions
{
    public static class PaginationExtensions
    {
        public static int PagesCount(int totalItems, int pageSize)
        {
            return (int)Math.Ceiling((double)totalItems / pageSize);
        }
        public static int PageNumber(int indexFromZero, int pageSize)
        {
            int page = (int)Math.Ceiling((double)(indexFromZero + 1) / pageSize);
            return page;
        }
        public static int PageNumber<T>(this IQueryable<T> query, int pageSize, long id) where T : EntityBase
        {
            var result = query.ToList().Select((x, i) => new { Item = x, Index = i }).FirstOrDefault(itemWithIndex => itemWithIndex.Item.Id == id);
            if (result != null)
            {
                return PageNumber(result.Index, pageSize);
            }
            else
            {
                return 1;
            }
        }
        public class PaginationResponse<T>
        {
            public int Page { get; }
            public int PageSize { get; }
            public List<T> PageItems { get; }
            public int ItemsTotal { get; }
            public int PagesCount { get; }
            public PaginationResponse(IQueryable<T> source, int page, int pageSize)
            {
                int total = source.Count();
                if (pageSize < 1) pageSize = 1;
                if (page < 1) page = 1;
                int maxPage = PagesCount(total, pageSize);
                if (page > maxPage) page = maxPage;
                if (page < 1) page = 1;
                var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                Page = page;
                PageSize = pageSize;
                PageItems = items ?? new List<T>();
                ItemsTotal = total;
                PagesCount = maxPage;
            }
        }
        public static PaginationResponse<T> PaginationRead<T>(this IQueryable<T> source, int page, int pageSize)
        {
            return new PaginationResponse<T>(source, page, pageSize);
        }
    }
}
