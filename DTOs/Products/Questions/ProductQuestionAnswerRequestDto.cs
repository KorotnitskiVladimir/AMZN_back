using System.ComponentModel.DataAnnotations;

namespace AMZN.DTOs.Products.Questions
{
    public class ProductQuestionAnswerRequestDto
    {
        [Required(ErrorMessage = "Text is required")]
        [StringLength(2048, MinimumLength = 5, ErrorMessage = "Text must be between 5 and 2048 characters")]
        public string Text { get; set; } = null!;
    }
}
