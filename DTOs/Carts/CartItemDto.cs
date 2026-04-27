namespace AMZN.DTOs.Carts;

public class CartItemDto
{
    public Guid ProductId { get; set; }
    public string ImageUrl { get; set; } = "";
    public string Title { get; set; } = "";
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}