namespace AMZN.DTOs.Products
{
    public class ProductRatingResponseDto
    {
        public decimal AverageRating { get; set; }
        public int RatingsCount { get; set; }
        public byte UserRating { get; set; }
    }
}
