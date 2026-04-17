using System.ComponentModel.DataAnnotations;

namespace AMZN.Models.Brands
{
    public class BrandCreateViewModel
    {
        private const int NameMaxLength = 64;


        [Required(ErrorMessage = "Name is required")]
        [StringLength(NameMaxLength, ErrorMessage = "Name is too long")]
        public string Name { get; set; } = "";

    }
}
