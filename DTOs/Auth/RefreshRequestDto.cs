using System.ComponentModel.DataAnnotations;

namespace AMZN.DTOs.Auth
{
    public class RefreshRequestDto
    {
        private const int TokenMaxLength = 512;

        [Display(Name = "Refresh token")]
        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        [StringLength(TokenMaxLength, ErrorMessage = "{0} must be {1} characters or less")]
        public string RefreshToken { get; set; } = "";
    }
}
