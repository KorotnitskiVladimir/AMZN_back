using AMZN.Shared.Exceptions;
using AMZN.Shared.Exceptions.Errors;
using System.Security.Claims;

namespace AMZN.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // Метод для API
        public static Guid GetRequiredUserId(this ClaimsPrincipal user)
        {
            string? userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);

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

        // Метод для MVC
        public static Guid? GetUserIdOrNull(this ClaimsPrincipal user)
        {
            string? userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdValue))
                return null;

            return Guid.TryParse(userIdValue, out Guid userId)
                ? userId
                : null;
        }

    }
}
