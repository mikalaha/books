using System.ComponentModel.DataAnnotations;

namespace BooksApi.Data
{
    public class Book : EntityBase
    {
        [Required]
        [MaxLength(256)]
        public string Title { get; set; } = null!;
        [Required]
        [MaxLength(256)]
        public string Author { get; set; } = null!;
        public int Year { get; set; }
        [Required]
        [MaxLength(16)]
        public string ISBN { get; set; } = null!;
        [Required]
        [MaxLength(512)]
        public string Description { get; set; } = null!;
    }
}
