using AMZN.DTOs.Common;
using AMZN.DTOs.Products;

namespace AMZN.DTOs.Home
{
    public class HomeResponseDto
    {
        public List<HomeCategoryCardDto> TopCategories { get; set; } = new();
        public List<HomeCategoryBlockDto> CategoryBlocks { get; set; } = new();

        public List<ProductCardDto> PopularProducts { get; set; } = new();
        public List<HomeCategoryCardDto> PopularCategories { get; set; } = new();

        public List<ProductCardDto> ProductsUnderTwenty { get; set; } = new();

        public List<ProductCardDto> MoreProducts { get; set; } = new();

    }
}
