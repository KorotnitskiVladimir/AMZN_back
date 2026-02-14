using System.Security.Claims;
using AMZN.Data;
using AMZN.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AMZN.Middleware;

public class AuthSessionMiddleware
{
    private readonly RequestDelegate _next;
    
    public AuthSessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, DataContext dataContext)
    {
        if (context.Request.Query.ContainsKey("logout"))
        {
            context.Session.Remove("userId");
            context.Response.Redirect(context.Request.Path);
        }
        if (context.Session.Keys.Contains("userId"))
        {
            context.Items.Add("auth", "OK");

            if (dataContext.UserRefreshTokens.FirstOrDefault(t =>
                    t.UserId.ToString() == context.Session.GetString("userId")) != null)
            {
                var user = dataContext.UserRefreshTokens.Include(t => t.User).FirstOrDefault(t => t.UserId.ToString() == context.Session.GetString("userId")).User;
                context.User = new ClaimsPrincipal(
                    new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Sid, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.FirstName),
                        new Claim(ClaimTypes.NameIdentifier, user.LastName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role.ToString())
                    }, nameof(AuthSessionMiddleware)));
            }
        }
        await _next(context);
    }
}

public static class AuthSessionMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthSession(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthSessionMiddleware>();
    }
}