namespace AMZN.Data.Entities
{
    public class ProductQuestion
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }

        public string Text { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }


        public Product Product { get; set; } = null!;
        public User User { get; set; } = null!;
        public ICollection<ProductQuestionAnswer> Answers { get; set; } = new List<ProductQuestionAnswer>();

    }
}
