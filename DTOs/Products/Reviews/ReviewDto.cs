namespace AMZN.DTOs.Products.Reviews
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public byte Rating { get; set; }
        public string Title { get; set; } = null!;
        public string Text { get; set; } = null!;
        public string AuthorName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
