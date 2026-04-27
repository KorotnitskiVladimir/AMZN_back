using AMZN.Data.Entities;

namespace AMZN.DTOs.Carts;

public class CartResponseDto
{
    public Guid CartId { get; set; }
    
    public List<CartItemDto>? Items { get; set; }
}