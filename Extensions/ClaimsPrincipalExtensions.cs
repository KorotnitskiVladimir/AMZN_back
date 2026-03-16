using AMZN.Shared.Exceptions;
using AMZN.Shared.Exceptions.Errors;
using System.Security.Claims;

namespace AMZN.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetRequiredUserId(this ClaimsPrincipal user)
        {
            var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                throw new ApiException(
                    ErrorCodes.AuthClaimsInvalid,
                    "Invalid auth claims",
                    StatusCodes.Status401Unauthorized
                );
            }

            return userId;
        }
    }
}
