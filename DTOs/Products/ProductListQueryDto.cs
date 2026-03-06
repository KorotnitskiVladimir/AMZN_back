
using System.ComponentModel.DataAnnotations;

namespace AMZN.DTOs.Products
{
    public class ProductListQueryDto : IValidatableObject
    {

        private const int PriceMax = 999_999_999;

        public Guid? CategoryId { get; set; }

        [Range(0, PriceMax)]
        public decimal? MaxPrice { get; set; }

        [Range(0, PriceMax)]
        public decimal? MinPrice { get; set; }

        [Range(0, 5)]
        public decimal? MinRating { get; set; }

        // featured, price_asc, price_desc, rating, newest
        public string? Sort {  get; set; }

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = 20;


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if( MinPrice.HasValue &&  MaxPrice.HasValue && MinPrice.Value > MaxPrice.Value)
            {
                yield return new ValidationResult(
                    "MinPrice must be <= MaxPrice",
                    new[] { nameof(MinPrice), nameof(MaxPrice) }
                    );
            }
        }

    }
}
