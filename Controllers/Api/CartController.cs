using AMZN.DTOs.Carts;
using AMZN.Services.Carts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using AMZN.Extensions;
namespace AMZN.Controllers.Api;

[Route("api/cart")]
[ApiController]

public class CartController : ControllerBase
{
    private readonly CartService _cartService;
    
    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }
    
    // POST api/cart/add
    [HttpPost("add")]
    [Authorize]
    [EnableRateLimiting("Auth")]
    public async Task<ActionResult<CartResponseDto>> AddToCart([FromRoute] string productId)
    {
        var userId = User.GetRequiredUserId();
        if (string.IsNullOrEmpty(productId))
        {
            return BadRequest("Product id is required");
        }

        if (!Guid.TryParse(productId, out Guid parsedId))
        {
            return BadRequest("Product id is invalid");
        }
        CartResponseDto response = await _cartService.AddToCartAsync(userId, parsedId);
        return StatusCode(StatusCodes.Status200OK, response);
    }
    
    // POST api/cart/remove
    [HttpPost("remove")]
    [Authorize]
    [EnableRateLimiting("Auth")]
    public async Task<ActionResult<CartResponseDto>> RemoveFromCart([FromRoute] string productId)
    {
        var userId = User.GetRequiredUserId();
        if (string.IsNullOrEmpty(productId))
        {
            return BadRequest("Product id is required");
        }


        if (!Guid.TryParse(productId, out Guid parsedId))
        {
            return BadRequest("Product id is invalid");
        }

        CartResponseDto response = await _cartService.RemoveFromCartAsync(userId, parsedId);
        return StatusCode(StatusCodes.Status200OK, response);
    }
    
    // POST api/cart/clear
    [HttpPost("clear")]
    [Authorize]
    [EnableRateLimiting("Auth")]
    public async Task<ActionResult<CartResponseDto>> ClearCart()
    {
        var userId = User.GetRequiredUserId();
        CartResponseDto response = await _cartService.ClearCartAsync(userId);
        return StatusCode(StatusCodes.Status200OK, response);
    }
    
    // POST api/cart/get
    [HttpPost("get")]
    [Authorize]
    [EnableRateLimiting("Auth")]
    public async Task<ActionResult<CartResponseDto>> GetCart()
    {
        var userId = User.GetRequiredUserId();
        CartResponseDto response = await _cartService.GetCartAsync(userId);
        return StatusCode(StatusCodes.Status200OK, response);
    }
    
    // POST api/cart/increaseQuantity
    [HttpPost("increaseQuantity")]
    [Authorize]
    [EnableRateLimiting("Auth")]
    public async Task<ActionResult<CartResponseDto>> IncreaseQuantity([FromRoute] string productId)
    {
        var userId = User.GetRequiredUserId();
        if (string.IsNullOrEmpty(productId))
        {
            return BadRequest("Product id is required");
        }
        
        if (!Guid.TryParse(productId, out Guid parsedId))
        {
            return BadRequest("Product id is invalid");
        }
        
        CartResponseDto response = await _cartService.IncreaseQuantityAsync(userId, parsedId);
        return StatusCode(StatusCodes.Status200OK, response);
    }
    
    // POST / api/cart/decreaseQuantity
    [HttpPost("decreaseQuantity")]
    [Authorize]
    [EnableRateLimiting("Auth")]
    public async Task<ActionResult<CartResponseDto>> DecreaseQuantity([FromRoute] string productId)
    {
        var userId = User.GetRequiredUserId();
        if (string.IsNullOrEmpty(productId))
        {
            return BadRequest("Product id is required");
        }

        if (!Guid.TryParse(productId, out Guid parsedId))
        {
            return BadRequest("Product id is invalid");
        }
        
        CartResponseDto response = await _cartService.DecreaseQuantityAsync(userId, parsedId);
        return StatusCode(StatusCodes.Status200OK, response);
    }
}