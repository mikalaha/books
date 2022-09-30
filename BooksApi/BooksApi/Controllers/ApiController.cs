using BooksApi.Data;
using BooksApi.Extensions;
using BooksApiClient.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BooksApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        readonly BooksDbContext dbContext;
        public ApiController(BooksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet("books")]
        public ActionResult Get(int? pageNumber, int pageSize, long? bookId, string? filterByTitle, string? filterByAuthor)
        {
            var books = dbContext.GetBooks(pageNumber, pageSize, bookId, filterByTitle, filterByAuthor);
            return Ok(books.ToDto());
        }
        [HttpGet("books/{id}")]
        public ActionResult Get(long id)
        {
            var book = dbContext.GetBook(id);
            if (book != null)
            {
                return Ok(book.ToDto());
            }
            return NotFound();
        }
        [HttpPost("books")]
        public ActionResult Post([FromBody]BookInformationDto book)
        {
            var added = dbContext.AddBook(book);
            if (added!=null)
            {
                ImageStorage.Put(added.Id, book.Image);
                return Ok(added.ToDto());
            }
            return StatusCode(500);
        }
        [HttpPut("books/{id}")]
        public ActionResult Put(long id, [FromBody] BookInformationDto book)
        {
            var updated = dbContext.UpdateBook(id, book);
            if (updated!=null)
            {
                if (book.Image!=null)
                {
                    ImageStorage.Put(id, book.Image);
                }
                return Ok(updated.ToDto());
            }
            return StatusCode(500);
        }
        [HttpDelete("books/{id}")]
        public ActionResult Delete(int id)
        {
            if (dbContext.DeleteBook(id))
            {
                ImageStorage.Delete(id);
                return Ok();
            }
            return NotFound();
        }
    }
}