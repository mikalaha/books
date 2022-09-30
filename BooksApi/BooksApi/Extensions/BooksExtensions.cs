using BooksApi.Data;
using BooksApiClient.Dto;

namespace BooksApi.Extensions
{
    public static class BooksExtensions
    {
        public static BookDto ToDto(this Book book)
        {
            return new BookDto()
            {
                Id = book.Id,
                Title = book.Title,
                Author=book.Author,
                Description=book.Description,
                ISBN=book.ISBN,
                Year=book.Year,
                Image = ImageStorage.Get(book.Id)
            };
        }
        public static List<BookDto> ToDto(this List<Book> books)
        {
            return books.Select(x => x.ToDto()).ToList();
        }
        public static BooksDto ToDto(this PaginationExtensions.PaginationResponse<Book> books)
        {
            return new BooksDto()
            {
                PagesCount = books.PagesCount,
                ItemsTotal = books.ItemsTotal,
                Page=books.Page,
                PageSize=books.PageSize,
                PageItems=books.PageItems.ToDto()
            };
        }
        public static Book ToData(this BookInformationDto dto, long id)
        {
            return new Book()
            {
                Author=dto.Author??string.Empty,
                Description=dto.Description??string.Empty,
                Id=id,
                ISBN=dto.ISBN??string.Empty,
                Title=dto.Title??string.Empty,
                Year=dto.Year??0
            };
        }
        public static void Update(Book target, BookInformationDto source)
        {
            if (!string.IsNullOrEmpty(source.Author)) target.Author = source.Author;
            if (!string.IsNullOrEmpty(source.Description)) target.Description = source.Description;
            if (!string.IsNullOrEmpty(source.ISBN)) target.ISBN = source.ISBN;
            if (!string.IsNullOrEmpty(source.Title)) target.Title = source.Title;
            if (source.Year!=null) target.Year = source.Year.Value;
        }
    }
}
