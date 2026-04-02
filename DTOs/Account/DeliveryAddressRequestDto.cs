using System.ComponentModel.DataAnnotations;

namespace AMZN.DTOs.Account;

public class DeliveryAddressRequestDto
{
    private const int NameMinLength = 3;
    private const int NameMaxLength = 64;
    private const int PostalCodeMinLength = 2;
    private const int PostalCodeMaxLength = 10;
    private const int CityMinLength = 2;
    private const int CityMaxLength = 32;
    private const string PersonNameRegex = @"^[\p{L}\p{M}]+(?:[ '\-][\p{L}\p{M}]+)*$";
    private const string AddressRegex = @"^[\w\s\d.,\-\/]{5,100}$";
    private const string PhoneRegex = @"^\+[1-9]\d{1,14}$";
    private const string CityRegex = @"^[\p{L}][\p{L}\s.'-]*[\p{L}]$";
    private const string CountryRegex = @"^[\p{L}][\p{L}\s.'()-]{1,60}[\p{L}.]$";
    private const string StateRegex = @"^[\p{L}0-9\s.'-]{2,32}$";
    
    [Display(Name = "First Name")]
    [Required]
    [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = "{0} must be {2}-{1} characters")]
    [RegularExpression(PersonNameRegex, ErrorMessage = "{0} may contain letters, spaces, hyphens and apostrophes")]
    public string FirstName { get; set; } = "";
    
    [Display(Name = "Last Name")]
    [Required]
    [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = "{0} must be {2}-{1} characters")]
    [RegularExpression(PersonNameRegex, ErrorMessage = "{0} may contain letters, spaces, hyphens and apostrophes")]
    public string LastName { get; set; } = "";
    
    [Display(Name = "Street Address")]
    [Required]
    [RegularExpression(AddressRegex, ErrorMessage = "{0} invalid format")]
    public string StreetAddress { get; set; } = "";
    
    [Display(Name = "City")]
    [Required]
    [StringLength(CityMaxLength, MinimumLength = CityMinLength, ErrorMessage = "{0} must be {2}-{1} characters")]
    [RegularExpression(CityRegex, ErrorMessage = "{0} invalid format")]
    public string City { get; set; } = "";

    [Display(Name = "Postal Code")]
    [Required]
    [StringLength(PostalCodeMaxLength, MinimumLength = PostalCodeMinLength, ErrorMessage = "{0} must be {2}-{1} characters}")]
    public string PostalCode { get; set; } = "";
    
    [Display(Name = "Country")]
    [Required]
    [RegularExpression(CountryRegex, ErrorMessage = "{0} invalid format")]
    public string Country { get; set; } = "";

    [Display(Name = "State")]
    [RegularExpression(StateRegex, ErrorMessage = "{0} invalid format")]
    public string? State { get; set; } = "";
    
    [Display(Name = "Phone number")]
    [Required]
    [RegularExpression(PhoneRegex, ErrorMessage = "Invalid {0} format")]
    public string PhoneNumber { get; set; } = "";
    
    [Display(Name = "Set as default delivery address")]
    public bool IsDefault { get; set; }
    
    
    
}