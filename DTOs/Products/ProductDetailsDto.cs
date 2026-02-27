using AMZN.DTOs.Common;

namespace AMZN.DTOs.Products
{

    public class ProductDetailsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public decimal Rating { get; set; }
        public int RatingCount { get; set; }

        public PriceDto Price { get; set; } = new();
        public ImageUrlDto PrimaryImage { get; set; } = new();
        public List<ImageDto> Images { get; set; } = new();

        public CategoryDto Category { get; set; } = new();

    }

}
