namespace BooksApiClient.Dto
{
    public class BookInformationDto
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public int? Year { get; set; }
        public string? ISBN { get; set; }
        public string? Description { get; set; }
        public byte[]? Image { get; set; }
    }
    public class BookDto : BookInformationDto
    {
        public long Id { get; set; }
    }
}
