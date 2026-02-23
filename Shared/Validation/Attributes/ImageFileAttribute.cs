using System.ComponentModel.DataAnnotations;
using AMZN.Shared.Validation.Files;

namespace AMZN.Shared.Validation.Attributes
{
    public sealed class ImageFileAttribute : ValidationAttribute
    {
        private readonly long _maxBytes;

        public ImageFileAttribute(long maxBytes)
        {
            _maxBytes = maxBytes;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is not IFormFile file)
                return new ValidationResult("Invalid file type");

            // Логику вынес в helper - ImageFileRules, чтобы тут не была простыня if-ов
            if (!ImageFileRules.TryValidate(file, _maxBytes, out var error))
            {
                var message = string.IsNullOrWhiteSpace(ErrorMessage) ? error : ErrorMessage;
                return new ValidationResult(message);
            }

            return ValidationResult.Success;
        }
    }
}