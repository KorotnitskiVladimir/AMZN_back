using System.Security.Claims;
using AMZN.Data;
using Microsoft.EntityFrameworkCore;

namespace AMZN.Middleware;

public class AuthTokenMiddleware
{
    private readonly RequestDelegate _next;
    
    public AuthTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, DataContext dataContext)
    {
        string authHeader = context.Request.Headers.Authorization.ToString();
        string? errorMessage = null;
        string scheme = "Bearer";
        Guid jti = default;
        if (string.IsNullOrEmpty(authHeader))
        {
            errorMessage = "Authorization header is required";
        }
        else if (!authHeader.StartsWith(scheme))
        {
            errorMessage = $"Authorization header must be {scheme}";
        }
        else
        {
            string credentials = authHeader[scheme.Length..].Trim();
            try
            {
                jti = Guid.Parse(credentials);
            }
            catch
            {
                errorMessage = "Authorization credentials must be a valid JWT";
            }
        }

        if (errorMessage == null)
        {
            var token = dataContext.UserRefreshTokens
                .Include(t => t.User)
                .FirstOrDefault(t => t.Id == jti);
            
            if (token == null)
            {
                errorMessage = "Credentials rejected";
            }
            
            else if (token.ExpiresAt < DateTime.Now)
            {
                errorMessage = "Token expired";
            }
            else
            {
                var user = token.User;
                context.User = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new Claim[]
                        {
                            new Claim(ClaimTypes.Sid, user.Id.ToString()),
                            new Claim(ClaimTypes.Name, user.FirstName),
                            new Claim(ClaimTypes.NameIdentifier, user.LastName),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Role, user.Role.ToString())
                        },
                        nameof(AuthTokenMiddleware)));
                context.Items.Add("UserRefreshToken", token);
            }
        }
        context.Items.Add(nameof(AuthTokenMiddleware), errorMessage);
        await _next(context);
    }
}

public static class AuthTokenMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthToken(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthTokenMiddleware>();
    }
}