using System.ComponentModel.DataAnnotations;

namespace AMZN.DTOs.Auth
{
    public class LoginRequestDto
    {

        private const int EmailMaxLength = 100;
        private const int PasswordMaxLength = 72;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "Invalid {0} format")]
        [StringLength(EmailMaxLength, ErrorMessage = "{0} must be {1} characters or less")]
        public string Email { get; set; } = "";


        [Display(Name = "Password")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(PasswordMaxLength, ErrorMessage = "{0} must be {1} characters or less")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

    }
}
