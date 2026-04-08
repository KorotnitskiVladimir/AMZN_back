namespace AMZN.DTOs.Home
{

    // Счетчики по категории для home page:
    // сколько товаров подходит под фильтр и сколько у этих товаров суммарно рейтинга
    public class CategoryProductCountsDto
    {
        public Guid CategoryId { get; set; }
        public int ProductCount { get; set; }
        public int RatingCount { get; set; }
    }

}
