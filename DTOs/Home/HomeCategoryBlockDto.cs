using AMZN.DTOs.Products;

namespace AMZN.DTOs.Home
{
    public class HomeCategoryBlockDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public List<ProductCardDto> Products { get; set; } = new();
    }
}
