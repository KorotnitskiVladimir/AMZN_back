using System.ComponentModel.DataAnnotations;

namespace AMZN.Shared.Validation.Attributes
{
    public sealed class RequiredFileAttribute : ValidationAttribute
    {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not IFormFile file || file.Length == 0)
            {
                return new ValidationResult(ErrorMessage ?? "File is required");
            }    
                
            return ValidationResult.Success;
        }

    }
}