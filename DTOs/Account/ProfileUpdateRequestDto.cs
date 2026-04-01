using System.ComponentModel.DataAnnotations;
namespace AMZN.DTOs.Auth;

public class ProfileUpdateRequestDto
{
    private const int NameMinLength = 3;
    private const int NameMaxLength = 64;
    private const int EmailMaxLength = 128;
    private const int PasswordMinLength = 6;
    private const int PasswordMaxLength = 72;
    private const string PersonNameRegex = @"^[\p{L}\p{M}]+(?:[ '\-][\p{L}\p{M}]+)*$";
    private const string PhoneRegex = @"^\+[1-9]\d{1,14}$";
    
    [Display(Name = "First name")]
    [Required]
    [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = "{0} must be {2}-{1} characters")]
    [RegularExpression(PersonNameRegex, ErrorMessage = "{0} may contain letters, spaces, hyphens and apostrophes")]
    public string FirstName { get; set; } = "";
    
    [Display(Name = "Last name")]
    [Required]
    [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = "{0} must be {2}-{1} characters")]
    [RegularExpression(PersonNameRegex, ErrorMessage = "{0} may contain letters, spaces, hyphens and apostrophes")]
    public string LastName { get; set; } = "";
    
    [Display(Name = "Email")]
    [Required]
    [EmailAddress(ErrorMessage = "Invalid {0} format")]
    [StringLength(EmailMaxLength, ErrorMessage = "{0} must be {1} characters or less")]
    public string Email { get; set; } = "";
    
    [Display(Name = "Phone number")]
    [StringLength(15, ErrorMessage = "{0} must be {1} characters or less")]
    [RegularExpression(PhoneRegex, ErrorMessage = "Invalid {0} format")]
    public string PhoneNumber { get; set; } = "";
    
    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateOnly? BirthDate { get; set; }
    
}