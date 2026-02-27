using AMZN.DTOs.Common;

namespace AMZN.DTOs.Home
{

    public class HomeResponseDto
    {
        public List<HomeProductDto> Products { get; set; } = new();
    }

    public class HomeProductDto
    {
        public Guid Id { get; set; }
        public decimal Rating { get; set; }     // шаг 0.5

        public PriceDto Price { get; set; } = new();
        public ImageUrlDto Image { get; set; } = new();

        public string Title { get; set; } = "";
    }

}
