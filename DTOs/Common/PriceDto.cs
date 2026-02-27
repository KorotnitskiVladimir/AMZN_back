namespace AMZN.DTOs.Common
{
    public class PriceDto
    {
        public decimal Current { get; set; }
        public decimal Original { get; set; }   // если скидки нет -> равно Current
    }
}
