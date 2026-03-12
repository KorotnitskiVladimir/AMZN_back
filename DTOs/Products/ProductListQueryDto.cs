
using System.ComponentModel.DataAnnotations;

namespace AMZN.DTOs.Products
{
    public class ProductListQueryDto : IValidatableObject
    {

        private const int PriceMax = 999_999_999;
        private const int PageSizeMax = 100;
        private const int MaxBrandIds = 50;

        private static readonly string[] AllowedSorts =
        {
            "featured",
            "price_asc",
            "price_desc",
            "rating",
            "rating_desc",
            "newest"
        };


        public Guid? CategoryId { get; set; }
        public List<Guid> BrandIds { get; set; } = new();

        [Range(0, PriceMax)]
        public decimal? MaxPrice { get; set; }

        [Range(0, PriceMax)]
        public decimal? MinPrice { get; set; }

        [Range(0, 5)]
        public decimal? MinRating { get; set; }

        public string? Sort {  get; set; }

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, PageSizeMax)]
        public int PageSize { get; set; } = 20;


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MinPrice.HasValue && MaxPrice.HasValue && MinPrice.Value > MaxPrice.Value)
                yield return new ValidationResult("MinPrice must be <= MaxPrice", new[] { nameof(MinPrice), nameof(MaxPrice) });

            if (BrandIds.Count > MaxBrandIds)
                yield return new ValidationResult($"Too many brands chosen (max {MaxBrandIds})", new[] { nameof(BrandIds) });

            if (!string.IsNullOrWhiteSpace(Sort) && !AllowedSorts.Contains(Sort.Trim(), StringComparer.OrdinalIgnoreCase))
                yield return new ValidationResult("Invalid sort value", new[] { nameof(Sort) });
        }


    }
}
