namespace AMZN.Models.Category;

public class CategoryViewModel
{
    public CategoryFormModel? FormModel { get; set; }
    public List<Data.Entities.Category> Categories { get; set; } = new();
}