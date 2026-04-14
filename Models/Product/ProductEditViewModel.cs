using AMZN.Data.Entities;

namespace AMZN.Models.Product
{
    public class ProductEditViewModel
    {
        public Guid ProductId { get; set; }

        public ProductEditFormModel Form { get; set; } = new();

        public string CurrentPrimaryImageUrl { get; set; } = null!;

        public List<ExistingProductImageViewModel> ExistingGalleryImages { get; set; } = new();

        public List<AMZN.Data.Entities.Category> Categories { get; set; } = new();
        public List<Brand> Brands { get; set; } = new();

        public string? ErrorMessage { get; set; }
    }
}
