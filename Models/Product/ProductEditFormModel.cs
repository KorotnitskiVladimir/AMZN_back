using AMZN.Shared.Validation.Attributes;
using AMZN.Shared.Validation.Files;
using System.ComponentModel.DataAnnotations;

namespace AMZN.Models.Product
{
    public class ProductEditFormModel : IValidatableObject
    {
        private const int TitleMaxLength = 256;
        private const int DescriptionMaxLength = 4000;

        private const string PriceMin = "0";
        private const string PriceMax = "999999999";

        private const long MaxImageBytes = 5 * 1024 * 1024; // 5 MB
        public const int MaxGalleryImages = 10;


        [Required(ErrorMessage = "Title is required")]
        [StringLength(TitleMaxLength, ErrorMessage = "Title is too long")]
        public string Title { get; set; } = null!;


        [StringLength(DescriptionMaxLength, ErrorMessage = "Description is too long")]
        public string? Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "StockQuantity must be >= 0")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        public Guid? BrandId { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public Guid? CategoryId { get; set; }


        [Range(typeof(decimal), PriceMin, PriceMax, ErrorMessage = "CurrentPrice must be between 0 and 999999999")]
        public decimal CurrentPrice { get; set; }


        [Range(typeof(decimal), PriceMin, PriceMax, ErrorMessage = "OriginalPrice must be between 0 and 999999999")]
        public decimal? OriginalPrice { get; set; }


        [ImageFile(MaxImageBytes, ErrorMessage = "Primary image is invalid")]
        public IFormFile? NewPrimaryImage { get; set; }

        // Новые gallery images, Добавляются после существующих
        public List<IFormFile>? NewGalleryImages { get; set; }

        // Порядок существующих gallery images после delete / reorder
        public List<Guid> ExistingGalleryImageIdsInOrder { get; set; } = new();


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OriginalPrice is > 0 && OriginalPrice < CurrentPrice)
            {
                yield return new ValidationResult(
                    "OriginalPrice must be >= CurrentPrice",
                    new[] { nameof(OriginalPrice), nameof(CurrentPrice) });
            }

            if (NewGalleryImages == null || NewGalleryImages.Count == 0)
                yield break;

            if (NewGalleryImages.Count > MaxGalleryImages)
            {
                yield return new ValidationResult(
                    "Too many gallery images (max 10)",
                    new[] { nameof(NewGalleryImages) });

                yield break;
            }

            foreach (var file in NewGalleryImages)
            {
                if (file == null)
                {
                    yield return new ValidationResult("Invalid file", new[] { nameof(NewGalleryImages) });
                    continue;
                }

                if (!ImageFileRules.TryValidate(file, MaxImageBytes, out var error))
                    yield return new ValidationResult(error, new[] { nameof(NewGalleryImages) });
            }
        }
    }
}
