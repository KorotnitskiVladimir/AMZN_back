using System.ComponentModel.DataAnnotations;

namespace AMZN.DTOs.Home
{
    public class HomeLastViewedQueryDto : IValidatableObject
    {
        private const int MaxLastViewedProductIds = 20;

        public List<Guid> ProductIds { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ProductIds.Count > MaxLastViewedProductIds)
            {
                yield return new ValidationResult($"Too many product id's. Maximum allowed: {MaxLastViewedProductIds}.", new[] { nameof(ProductIds) });
            }
        }
    }
}
