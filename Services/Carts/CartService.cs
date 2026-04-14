using AMZN.Data.Entities;
using AMZN.DTOs.Carts;
using AMZN.Repositories.Carts;
using AMZN.Repositories.Products;
using AMZN.Repositories.Users;
using AMZN.Shared.Exceptions;
using AMZN.Shared.Exceptions.Errors;
using AMZN.Shared.Transactions;

namespace AMZN.Services.Carts;

public class CartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    
    public CartService(ICartRepository cartRepository,
        IProductRepository productRepository,
        IUserRepository userRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
    }

    public async Task<CartResponseDto> AddToCartAsync(Guid userId, Guid productId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new ApiException(ErrorCodes.UserNotFound, "User not found", StatusCodes.Status404NotFound);
        }
        
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            throw new ApiException(ErrorCodes.ProductNotFound, "Product not found", StatusCodes.Status404NotFound);
        }
        
        var cart = await _cartRepository.GetCartByUserIdAsync(user.Id);
        if (cart == null)
        {
            cart = new() 
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                User = user,
                IsActive = true,
                Items = new List<CartItem>()
            };
            await _cartRepository.AddCartAsync(cart);
            CartItem cartItem = new()
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductId = productId,
                Product = product,
                Cart = cart,
                Quantity = 1
            };
            await _cartRepository.AddCartItemAsync(cartItem);
        }
        if (cart.IsActive)
        {
            if (await _cartRepository.IsCartEmptyAsync(cart.Id))
            {
                CartItem cartItem = new()
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = productId,
                    Product = product,
                    Cart = cart,
                    Quantity = 1
                };
                await _cartRepository.AddCartItemAsync(cartItem);
            }
            var item = await _cartRepository.IsItemInCartAsync(productId, cart.Id);
            if (item == null)
            {
                item = new()
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = productId,
                    Product = product,
                    Cart = cart,
                    Quantity = 1
                };
                await _cartRepository.AddCartItemAsync(item);
            }
            item.Quantity++;
            await _cartRepository.UpdateCartItemAsync(item);
        }
        List<Product> products = new List<Product>();
        foreach (var ci in cart.Items)
        {
            products.Add(ci.Product);
        }
        return new CartResponseDto()
        {
            Cart = cart,
            Products = products
        };
    }
}