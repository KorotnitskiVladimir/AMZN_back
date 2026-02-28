namespace AMZN.Data.Entities;

public class ProductAction
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid ActionId { get; set; }
    
    public Product Product { get; set; } = null!;
    public Action Action { get; set; } = null!;
}