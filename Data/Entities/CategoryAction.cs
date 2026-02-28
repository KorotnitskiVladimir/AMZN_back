namespace AMZN.Data.Entities;

public class CategoryAction
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public Guid ActionId { get; set; } 
    public Category Category { get; set; } = null!;
    public Action Action { get; set; } = null!;
}