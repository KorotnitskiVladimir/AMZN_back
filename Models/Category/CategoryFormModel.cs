using Microsoft.AspNetCore.Mvc;

namespace AMZN.Models.Category;

public class CategoryFormModel
{
    [FromForm(Name = "category-name")]
    public string Name { get; set; } = null!;
    [FromForm(Name = "parent-category")]
    public string? ParentCategory { get; set; }
    [FromForm(Name = "category-description")]
    public string Description { get; set; } = null!;
    [FromForm(Name = "category-image")]
    public IFormFile Image { get; set; } = null!;
}