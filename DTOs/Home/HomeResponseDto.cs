using AMZN.DTOs.Common;
using AMZN.DTOs.Products;

namespace AMZN.DTOs.Home
{

    public class HomeResponseDto
    {
        public List<ProductCardDto> Products { get; set; } = new();
    }



}
