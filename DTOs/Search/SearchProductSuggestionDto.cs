using AMZN.DTOs.Common;

namespace AMZN.DTOs.Search
{
    public class SearchProductSuggestionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public PriceDto Price { get; set; } = new();
        public ImageUrlDto Image { get; set; } = new();
        public string BrandName { get; set; } = "";
        public string CategoryName { get; set; } = "";
    }
}
