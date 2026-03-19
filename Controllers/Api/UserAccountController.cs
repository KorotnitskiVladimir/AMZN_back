using AMZN.DTOs.Auth;
using AMZN.Services.Auth;
using AMZN.Shared.Exceptions;
using AMZN.Shared.Exceptions.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using AMZN.Extensions;
using AMZN.Services.Account;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AMZN.Controllers.Api;

[Route("api/user")]
[ApiController]
public class UserAccountController : ControllerBase
{
    private readonly AccountService _accountService;

    public UserAccountController(
        AccountService accountService)
    {
        _accountService = accountService;
    }
    // POST api/user/update
    [HttpPost("update")]
    [Authorize]
    [EnableRateLimiting("Auth")]
    public async Task<ActionResult<ProfileUpdateResponseDto>> Update([FromBody] ProfileUpdateRequestDto request)
    {
        var userId = User.GetRequiredUserId();
        //string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        /*if (userId == Guid.Empty)
        {
            throw new ApiException(ErrorCodes.AuthClaimsInvalid, "Invalid auth claims", StatusCodes.Status401Unauthorized);
        }*/
        ProfileUpdateResponseDto response = await _accountService.UpdateProfileAsync(request, userId);
        return StatusCode(StatusCodes.Status200OK, response);
    }
    
    // POST api/user/delete
    
    [HttpPost("delete")]
    [Authorize]
    [EnableRateLimiting("Auth")]
    public async Task<ActionResult> DeleteUser()
    {
        //string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = User.GetRequiredUserId();
        /*if (userId == Guid.Empty)
        {
            throw new ApiException(ErrorCodes.AuthClaimsInvalid, "Invalid auth claims", StatusCodes.Status401Unauthorized);
        }*/
        await _accountService.DeleteUserAsync(userId);
        return StatusCode(StatusCodes.Status200OK);
    }
    
    
}