using System.ComponentModel.DataAnnotations;

namespace AMZN.DTOs.Auth;

public class PaymentMethodRequestDto
{
    private const string CardRegex = @"^\d{13,19}?$";
    private const string CvvRegex = @"^\d{3,4}$";
    
    [Display(Name = "Card Number")]
    [Required]
    [RegularExpression(CardRegex, ErrorMessage = "Invalid {0} format")]
    public string CardNumber { get; set; } = "";
    
    [Display(Name = "CVV")]
    [Required]
    [RegularExpression(CvvRegex, ErrorMessage = "Invalid {0} format")]
    public string Cvv { get; set; } = "";
    
    [Display(Name = "Expiration Date")]
    [Required]
    public DateOnly ExpirationDate { get; set; }
    
    [Display(Name = "Set as default payment method")]
    public bool IsDefault { get; set; }
}