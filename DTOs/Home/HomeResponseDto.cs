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
        public ImageDto Image { get; set; } = new();

        public string Name { get; set; } = "";
    }

    public class PriceDto
    {
        public decimal Current { get; set; }
        public decimal Original { get; set; }   // если скидки нет -> равно Current
    }

    public class ImageDto
    {
        public string Url { get; set; } = "";
    }

}
