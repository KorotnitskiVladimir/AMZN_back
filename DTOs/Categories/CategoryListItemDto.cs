namespace AMZN.DTOs.Categories
{
    public class CategoryListItemDto
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public bool HasChildren { get; set; }
    }
}
