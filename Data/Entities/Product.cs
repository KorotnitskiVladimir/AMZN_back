namespace AMZN.Data.Entities
{
    public class Product
    {

        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }

        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public decimal CurrentPrice { get; set; }
        public decimal? OriginalPrice { get; set; }             // null ->  скидки нет

        public string PrimaryImageUrl { get; set; } = null!;    // главная картинка продукта, что-бы не делать join ради одной фотки

        public int RatingSum { get; set; } = 0;
        public int RatingCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        //
        public Category Category { get; set; } = null!;
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductRating> Ratings { get; set; } = new List<ProductRating>();

    }
}
