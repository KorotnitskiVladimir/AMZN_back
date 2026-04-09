using AMZN.Data.Entities;

namespace AMZN.DTOs.Carts;

public class CartResponseDto
{
    public Cart? Cart { get; set; }
    
    public List<Product>? Products { get; set; }
}