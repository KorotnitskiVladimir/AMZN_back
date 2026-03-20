namespace AMZN.Data.Entities
{
    public class Product
    {

        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }

        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int StockQuantity { get; set; }

        public decimal CurrentPrice { get; set; }
        public decimal? OriginalPrice { get; set; }             // null ->  скидки нет

        public string PrimaryImageUrl { get; set; } = null!;

        public int RatingSum { get; set; } = 0;
        public int RatingCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        //
        public Guid SellerId { get; set; }
        public User Seller { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductRating> Ratings { get; set; } = new List<ProductRating>();
        public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        public Guid BrandId { get; set; }
        public Brand Brand { get; set; } = null!;

    }
}
