using System.ComponentModel.DataAnnotations;

namespace AMZN.DTOs.Products
{
    public class SetProductRatingRequestDto
    {
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public byte Rating { get; set; }
    }
}
