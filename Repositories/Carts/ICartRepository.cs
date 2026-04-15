using AMZN.Data.Entities;

namespace AMZN.Repositories.Carts;

public interface ICartRepository
{
    Task AddCartAsync(Cart cart);
    
    Task<Cart?> GetCartAsync(Guid cartId);
    
    Task<bool> IsCartExistsAsync(Guid cartId);
    
    Task<Cart?> GetCartByUserIdAsync(Guid userId);
    Task<bool> IsCartEmptyAsync(Guid cartId);
    Task UpdateCartAsync(Cart cart);
    
    Task AddCartItemAsync(CartItem cartItem);
    
    Task RemoveCartItemAsync(CartItem cartItem);
    
    Task DeleteCartAsync(Cart cart);
    
    Task<List<CartItem>> GetCartItemsAsync(Guid cartId);
    
    Task<CartItem?> IsItemInCartAsync(Guid productId, Guid cartId);

    Task UpdateCartItemAsync(CartItem cartItem);
    
    Task<CartItem?> GetCartItemAsync(Guid productId, Guid cartId);

}