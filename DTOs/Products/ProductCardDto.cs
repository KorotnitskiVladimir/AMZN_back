using AMZN.DTOs.Common;

namespace AMZN.DTOs.Products
{
    public class ProductCardDto
    {
        public Guid Id { get; set; }
        public decimal Rating { get; set; }
        public PriceDto Price { get; set; } = new();
        public ImageUrlDto Image { get; set; } = new();
        public string Title { get; set; } = "";

    }
}
