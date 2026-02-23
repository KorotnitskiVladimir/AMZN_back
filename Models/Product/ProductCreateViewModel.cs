using AMZN.Data.Entities;

namespace AMZN.Models.Product
{
    public class ProductCreateViewModel
    {
        public ProductCreateFormModel Form { get; set; } = new();
        public List<AMZN.Data.Entities.Category> Categories { get; set; } = new();

        public string? ErrorMessage { get; set; }
    }
}
