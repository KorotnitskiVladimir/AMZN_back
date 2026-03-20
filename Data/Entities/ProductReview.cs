namespace AMZN.Data.Entities
{

    public class ProductReview
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
