using AMZN.Data;
using AMZN.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AMZN.Repositories.Carts;

public class CartRepository : ICartRepository
{
    
    private readonly DataContext _dataContext;

    public CartRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    public async Task AddCartAsync(Cart cart)
    {
        await _dataContext.Carts.AddAsync(cart);
        await _dataContext.SaveChangesAsync();
    }

    public async Task<Cart?> GetCartAsync(Guid cartId)
    {
        return await _dataContext.Carts.FirstOrDefaultAsync(c => c.Id == cartId);
    }
    
    public async Task<bool> IsCartExistsAsync(Guid cartId)
    {
        return await _dataContext.Carts.AnyAsync(c => c.Id == cartId);
    }
    
    public async Task<Cart?> GetCartByUserIdAsync(Guid userId)
    {
        return await _dataContext.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
    }
    
    public async Task<bool> IsCartEmptyAsync(Guid cartId)
    {
        return !(await _dataContext.CartItems.AnyAsync(ci => ci.CartId == cartId));
    }
    
    public async Task UpdateCartAsync(Cart cart)
    {
        _dataContext.Carts.Update(cart);
        await _dataContext.SaveChangesAsync();
    }

    public Task AddCartItemAsync(CartItem cartItem)
    {
        _dataContext.CartItems.Add(cartItem);
        var cart = cartItem.Cart;
        cart.Items.Add(cartItem);
        _dataContext.Carts.Update(cart);
        return _dataContext.SaveChangesAsync();
    }
    
    public Task RemoveCartItemAsync(CartItem cartItem)
    {
        var cart = cartItem.Cart;
        cart.Items.Remove(cartItem);
        _dataContext.Carts.Update(cart);
        _dataContext.CartItems.Remove(cartItem);
        return _dataContext.SaveChangesAsync();
    }
    
    public async Task DeleteCartAsync(Cart cart)
    {
        _dataContext.Carts.Remove(cart);
        await _dataContext.SaveChangesAsync();
    }
    
    public async Task<List<CartItem>> GetCartItemsAsync(Guid cartId)
    {
        return await _dataContext.CartItems.Where(ci => ci.CartId == cartId).ToListAsync();
    }
    
    public async Task<CartItem?> IsItemInCartAsync(Guid productId, Guid cartId)
    {
        return await _dataContext.CartItems.FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.CartId == cartId);
    }
    
    public async Task UpdateCartItemAsync(CartItem cartItem)
    {
        _dataContext.CartItems.Update(cartItem);
        await _dataContext.SaveChangesAsync();
    }
    
    public async Task<CartItem?> GetCartItemAsync(Guid productId, Guid cartId)
    {
        return await _dataContext.CartItems.FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.CartId == cartId);
    }

    public async Task<List<User>?> GetUsersWithoutCartAsync()
    {
        List<User> users = new List<User>();
        List<Guid> userId = await _dataContext.Carts.Select(c => c.UserId).ToListAsync();
        return await _dataContext.Users.Where(u => !userId.Contains(u.Id)).ToListAsync();
    }

    public async Task ClearCartAsync(Guid cartId)
    {
        var cart = await _dataContext.Carts.FirstOrDefaultAsync(c => c.Id == cartId);
        if (cart == null) return;
        var items = await _dataContext.CartItems.Where(ci => ci.CartId == cartId).ToListAsync();
        foreach (var item in items)
        {
            cart.Items.Remove(item);
            _dataContext.CartItems.Remove(item);
        }
        await _dataContext.SaveChangesAsync();
    }
}