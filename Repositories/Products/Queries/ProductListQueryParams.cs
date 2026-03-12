
namespace AMZN.Repositories.Products.Queries
{
    public class ProductListQueryParams
    {
        public Guid? CategoryId { get; set; }
        public List<Guid> BrandIds { get; set; } = new();

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinRating { get; set; }

        public string? Sort {  get; set; }

    }

}
