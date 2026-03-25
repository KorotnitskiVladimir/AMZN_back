namespace AMZN.DTOs.Products.Questions
{
    public class ProductQuestionDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public string AuthorName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int AnswersCount { get; set; }

        public List<ProductQuestionAnswerDto> PreviewAnswers { get; set; } = new();
    }
}
