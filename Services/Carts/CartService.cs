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
        User? user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new ApiException(ErrorCodes.UserNotFound, "User not found", StatusCodes.Status404NotFound);
        }
        
        Product? product = await _productRepository.GetByIdAsync(productId);
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
        else
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
        List<CartItemDto> items = new List<CartItemDto>();
        foreach (var ci in cart.Items)
        {
            items.Add(new CartItemDto()
            {
                ProductId = ci.ProductId,
                ImageUrl = ci.Product.PrimaryImageUrl,
                Title = ci.Product.Title,
                Quantity = ci.Quantity,
                Price = ci.Product.CurrentPrice
            });
        }
        return new CartResponseDto()
        {
            CartId = cart.Id,
            Items = items
        };
    }

    public async Task<CartResponseDto> RemoveFromCartAsync(Guid userId, Guid productId)
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
            throw new ApiException(ErrorCodes.CartNotFound, "Cart not found", StatusCodes.Status404NotFound);
        }
        
        var item = await _cartRepository.GetCartItemAsync(cart.Id, productId);
        if (item == null)
        {
            throw new ApiException(ErrorCodes.ProductNotFound, "Product not found", StatusCodes.Status404NotFound);
        }

        await _cartRepository.RemoveCartItemAsync(item);
        
        List<CartItemDto> items = new List<CartItemDto>();
        foreach (var ci in cart.Items)
        {
            items.Add(new CartItemDto()
            {
                ProductId = ci.ProductId,
                ImageUrl = ci.Product.PrimaryImageUrl,
                Title = ci.Product.Title,
                Quantity = ci.Quantity,
                Price = ci.Product.CurrentPrice
            });
        }
        return new CartResponseDto()
        {
            CartId = cart.Id,
            Items = items
        };
    }

    public async Task<CartResponseDto> ClearCartAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new ApiException(ErrorCodes.UserNotFound, "User not found", StatusCodes.Status404NotFound);
        }
        
        var cart = await _cartRepository.GetCartByUserIdAsync(user.Id);
        if (cart == null)
        {
            throw new ApiException(ErrorCodes.CartNotFound, "Cart not found", StatusCodes.Status404NotFound);
        }

        if (await _cartRepository.IsCartEmptyAsync(cart.Id))
        {
            throw new ApiException(ErrorCodes.ProductNotFound, "Cart is empty", StatusCodes.Status404NotFound);
        }

        foreach (var ci in cart.Items)
        {
            await _cartRepository.RemoveCartItemAsync(ci);
        }
        
        List<CartItemDto> items = new List<CartItemDto>();
        
        return new CartResponseDto()
        {
            CartId = cart.Id,
            Items = items,
        };
    }
    
    public async Task<CartResponseDto> GetCartAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new ApiException(ErrorCodes.UserNotFound, "User not found", StatusCodes.Status404NotFound);
        }
        
        var cart = await _cartRepository.GetCartByUserIdAsync(user.Id);
        if (cart == null)
        {
            throw new ApiException(ErrorCodes.CartNotFound, "Cart not found", StatusCodes.Status404NotFound);
        }
        
        List<CartItemDto> items = new List<CartItemDto>();
        foreach (var ci in cart.Items)
        {
            items.Add(new CartItemDto()
            {
                ProductId = ci.ProductId,
                ImageUrl = ci.Product.PrimaryImageUrl,
                Title = ci.Product.Title,
                Quantity = ci.Quantity,
                Price = ci.Product.CurrentPrice
            });
        }
        return new CartResponseDto()
        {
            CartId = cart.Id,
            Items = items
        };     
    }

    public async Task<CartResponseDto> IncreaseQuantityAsync(Guid userId, Guid productId)
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
            throw new ApiException(ErrorCodes.CartNotFound, "Cart not found", StatusCodes.Status404NotFound);
        }
        
        var item = await _cartRepository.GetCartItemAsync(cart.Id, productId);
        if (item == null)
        {
            throw new ApiException(ErrorCodes.ProductNotFound, "Product not found", StatusCodes.Status404NotFound);
        }
        
        item.Quantity++;
        await _cartRepository.UpdateCartItemAsync(item);
        
        List<CartItemDto> items = new List<CartItemDto>();
        foreach (var ci in cart.Items)
        {
            items.Add(new CartItemDto()
            {
                ProductId = ci.ProductId,
                ImageUrl = ci.Product.PrimaryImageUrl,
                Title = ci.Product.Title,
                Quantity = ci.Quantity,
                Price = ci.Product.CurrentPrice
            });
        }
        return new CartResponseDto()
        {
            CartId = cart.Id,
            Items = items
        };  
    }

    public async Task<CartResponseDto> DecreaseQuantityAsync(Guid userId, Guid productId)
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
            throw new ApiException(ErrorCodes.CartNotFound, "Cart not found", StatusCodes.Status404NotFound);
        }
        
        var item = await _cartRepository.GetCartItemAsync(cart.Id, productId);
        if (item == null)
        {
            throw new ApiException(ErrorCodes.ProductNotFound, "Product not found", StatusCodes.Status404NotFound);
        }
        
        item.Quantity--;
        if (item.Quantity <= 0)
        {
            await _cartRepository.RemoveCartItemAsync(item);
        }
        else
        {
            await _cartRepository.UpdateCartItemAsync(item);
        }
        
        List<CartItemDto> items = new List<CartItemDto>();
        foreach (var ci in cart.Items)
        {
            items.Add(new CartItemDto()
            {
                ProductId = ci.ProductId,
                ImageUrl = ci.Product.PrimaryImageUrl,
                Title = ci.Product.Title,
                Quantity = ci.Quantity,
                Price = ci.Product.CurrentPrice
            });
        }
        return new CartResponseDto()
        {
            CartId = cart.Id,
            Items = items
        };
    }

    public async Task CreateCartsIfNotExist()
    {
        var users = _cartRepository.GetUsersWithoutCartAsync().Result;
        if (users == null)
        {
            return;
        }
        foreach (var user in users)
        {
           Cart cart = new()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                User = user,
                Items = new List<CartItem>()
            };
            await _cartRepository.AddCartAsync(cart);
        }
    }
    
}