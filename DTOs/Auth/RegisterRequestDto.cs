using System.ComponentModel.DataAnnotations;

namespace AMZN.DTOs.Auth
{
    public class RegisterRequestDto
    {
        private const int NameMinLength = 2;
        private const int NameMaxLength = 50;
        private const int EmailMaxLength = 100;
        private const int PasswordMinLength = 6;
        private const int PasswordMaxLength = 64;


        [Display(Name = "First name")]
        [Required(ErrorMessage = "{0} is required")]     // {0} в ErrorMessage — плейсхолдер для имени поля из [Display] (выше) или имени свойства, упрощает локализацию и переиспользование сообщений об ошибках.
        [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = "{0} must be {2}-{1} characters")]
        public string FirstName { get; set; } = "";


        [Display(Name = "Last name")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = "{0} must be {2}-{1} characters")]
        public string LastName { get; set; } = "";


        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "Invalid {0} format")]
        [StringLength(EmailMaxLength, MinimumLength = 5, ErrorMessage = "{0} must be 5-{1} characters")]
        public string Email { get; set; } = "";


        [Display(Name = "Password")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(PasswordMaxLength, MinimumLength = PasswordMinLength, ErrorMessage = "{0} must be {2}-{1} characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "{0} must include uppercase, lowercase, and a number")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";


        [Display(Name = "Password Repeat")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(PasswordMaxLength, MinimumLength = PasswordMinLength, ErrorMessage = "{0} must be {2}-{1} characters")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string PasswordRepeat { get; set; } = "";



    }
}
