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
}