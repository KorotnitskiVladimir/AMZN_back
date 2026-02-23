using System.ComponentModel.DataAnnotations;
using AMZN.Shared.Validation.Attributes;
using AMZN.Shared.Validation.Files;

namespace AMZN.Models.Product
{
    public class ProductCreateFormModel : IValidatableObject
    {
        private const int TitleMaxLength = 256;
        private const int DescriptionMaxLength = 4000;

        private const string PriceMin = "0";
        private const string PriceMax = "999999999";    

        private const long MaxImageBytes = 5 * 1024 * 1024;     // 5MB
        public const int MaxGalleryImages = 10;



        [Required(ErrorMessage = "Title is required")]
        [StringLength(TitleMaxLength, ErrorMessage = "Title is too long")]
        public string Title { get; set; } = null!;


        [StringLength(DescriptionMaxLength, ErrorMessage = "Description is too long")]
        public string? Description { get; set; }


        [Required(ErrorMessage = "Category is required")]
        public Guid? CategoryId { get; set; }


        [Range(typeof(decimal), PriceMin, PriceMax, ErrorMessage = "CurrentPrice must be between 0 and 999999999")]
        public decimal CurrentPrice { get; set; }


        [Range(typeof(decimal), PriceMin, PriceMax, ErrorMessage = "OriginalPrice must be between 0 and 999999999")]
        public decimal? OriginalPrice { get; set; }


        [RequiredFile(ErrorMessage = "Primary image is required")]
        [ImageFile(MaxImageBytes, ErrorMessage = "Primary image is invalid")]
        public IFormFile? PrimaryImage { get; set; }


        public List<IFormFile>? Images { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // OriginalPrice >= CurrentPrice
            if (OriginalPrice is > 0 && OriginalPrice < CurrentPrice)
            {
                yield return new ValidationResult(
                    "OriginalPrice must be >= CurrentPrice",
                    new[] { nameof(OriginalPrice), nameof(CurrentPrice) }
                );
            }

            if (Images == null || Images.Count == 0)
                yield break;

            // Галерея до 10 img
            if (Images.Count > MaxGalleryImages)
            {
                yield return new ValidationResult("Too many gallery images (max 10)", new[] { nameof(Images) });
                yield break;
            }

            // Проверка файлов галереи
            foreach (var file in Images)
            {
                if (file == null)
                {
                    yield return new ValidationResult("Invalid file", new[] { nameof(Images) });
                    continue;
                }

                if (!ImageFileRules.TryValidate(file, MaxImageBytes, out var error))
                    yield return new ValidationResult(error, new[] { nameof(Images) });
            }


        }

    }
}