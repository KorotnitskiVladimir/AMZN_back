namespace AMZN.DTOs.Products.Questions
{
    public class ProductQuestionAnswerDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public string AuthorName { get; set; } = null!;
        public bool IsSellerAnswer { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
