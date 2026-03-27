using AMZN.DTOs.Brands;
using AMZN.DTOs.Common;

namespace AMZN.DTOs.Search
{
    public class SearchSuggestionsResponseDto
    {
        public List<SearchProductSuggestionDto> Products { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
        public List<BrandDto> Brands { get; set; } = new();
    }
}
