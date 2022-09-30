using System.ComponentModel.DataAnnotations;

namespace BooksApi.Data
{
    public class EntityBase
    {
        [Key]
        public long Id { get; set; }
    }
}
