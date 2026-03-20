using System.ComponentModel.DataAnnotations;

namespace AMZN.DTOs.Products.Reviews
{
    public class ReviewRequestDto
    {
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public byte Rating { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(120, ErrorMessage = "Title is too long")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Text is required")]
        [MaxLength(4000, ErrorMessage = "Text is too long")]
        public string Text { get; set; } = null!;
    }
}
