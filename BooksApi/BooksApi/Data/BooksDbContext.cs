using BooksApi.Extensions;
using BooksApiClient.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksApi.Data
{
    public class BooksDbContext : DbContext
    {
        public static string DbFolder { get; }
        public static string DbPath { get; }
        static BooksDbContext()
        {
            DbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Books");
            if (!Directory.Exists(DbFolder))
            {
                Directory.CreateDirectory(DbFolder);
            }
            DbPath = Path.Combine(DbFolder, "books.db");

        }
        public virtual DbSet<Book> Books { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath};");
        }

        public Book? GetBook(long id)
        {
            return Books.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }
        public PaginationExtensions.PaginationResponse<Book> GetBooks(int? pageNumber, int pageSize, long? bookId, string? filterByTitle, string? filterByAuthor)
        {
            var books = Books.AsNoTracking().OrderBy(x => x.Author).AsQueryable();
            if (!string.IsNullOrEmpty(filterByTitle))
            {
                books = books.Where(x => EF.Functions.Like(x.Title, $"%{filterByTitle}%"));
            }
            if (!string.IsNullOrEmpty(filterByAuthor))
            {
                books = books.Where(x => EF.Functions.Like(x.Author, $"%{filterByAuthor}%"));
            }
            if (pageNumber == null)
            {
                if (bookId != null)
                {
                    pageNumber = books.PageNumber(pageSize, bookId.Value);
                }
                else
                {
                    pageNumber = 1;
                }
            }
            return books.PaginationRead(pageNumber.Value, pageSize);
        }
        public Book? AddBook(BookInformationDto dto)
        {
            try
            {
                var book = dto.ToData(0);
                Books.Add(book);
                SaveChanges();
                return book;
            }
            catch
            {
            }
            return null;
        }
        public Book? UpdateBook(long id, [FromBody] BookInformationDto book)
        {
            try
            {
                var exist = Books.FirstOrDefault(x => x.Id == id);
                if (exist!=null)
                {
                    BooksExtensions.Update(exist, book);
                    SaveChanges();
                    return exist;
                }
            }
            catch
            {
            }
            return null;
        }
        public bool DeleteBook(int id)
        {
            try
            {
                var book = Books.FirstOrDefault(x => x.Id == id);
                if (book!=null)
                {
                    Books.Remove(book);
                    SaveChanges();
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }
    }
}
