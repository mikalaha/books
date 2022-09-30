namespace BooksApiClient.Dto
{
    public class BooksDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<BookDto>? PageItems { get; set; }
        public int ItemsTotal { get; set; }
        public int PagesCount { get; set; }

    }
}
