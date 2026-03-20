using System.ComponentModel.DataAnnotations;

namespace AMZN.DTOs.Products.Reviews
{
    public class ReviewParamsDto : IValidatableObject
    {
        private const int PageSizeMax = 50;

        private static readonly string[] AllowedSorts =
        {
            "top",
            "recent"
        };

        public string? Sort { get; set; } = "top";

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, PageSizeMax)]
        public int PageSize { get; set; } = 10;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(Sort) &&
                !AllowedSorts.Contains(Sort.Trim(), StringComparer.OrdinalIgnoreCase))
            {
                yield return new ValidationResult("Invalid sort value", new[] { nameof(Sort) });
            }
        }
    }
}
