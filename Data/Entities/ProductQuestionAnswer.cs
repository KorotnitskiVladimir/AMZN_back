namespace AMZN.Data.Entities
{
    public class ProductQuestionAnswer
    {
        public Guid Id { get; set; }

        public Guid QuestionId { get; set; }
        public Guid UserId { get; set; }

        public string Text { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }


        public ProductQuestion Question { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
