using System.Buffers.Text;
using System.ComponentModel;
using AMZN.Data.Entities;
using AMZN.Models;
using AMZN.Security.Passwords;
using AMZN.Security.Tokens;
using Microsoft.AspNetCore.Authentication;

namespace AMZN.Data;

public class DataAccessor
{
    private readonly DataContext _dataContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJwtService _jwt;
    
    public DataAccessor(DataContext dataContext, 
        IPasswordHasher passwordHasher, 
        IHttpContextAccessor httpContextAccessor,
        IJwtService jwt)
    {
        _dataContext = dataContext;
        _passwordHasher = passwordHasher;
        _httpContextAccessor = httpContextAccessor;
        _jwt = jwt;
    }

    public UserRefreshToken Authenticate(HttpRequest request)
    {
        string authHeader = request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader))
        {
            throw new Win32Exception(401, "Authorization header is required");
        }
        
        string scheme = "Bearer ";
        if (!authHeader.StartsWith(scheme))
        {
            throw new Win32Exception(401, $"Authorization header must be {scheme}");
        }
        
        string credentials = authHeader[scheme.Length..].Trim();
        string authData;
        try
        {
            authData = System.Text.Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(credentials));
        }
        catch
        {
            throw new Win32Exception(401, $"Not valid Base64 code '{credentials}'");
        }
        string[] authParts = authData.Split(':');
        if (authParts.Length != 2)
        {
            throw new Win32Exception(401, "Invalid credentials format. Expected 'email:password'");
        }
        
        string email = authParts[0];
        string password = authParts[1];
        var user = _dataContext.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            throw new Win32Exception(401, "User not found");
        }

        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            throw new Win32Exception(401, "Invalid password");
        }
        
        var token = _dataContext.UserRefreshTokens.FirstOrDefault(t => t.UserId == user.Id);
        if (token != null && token.IsRevoked)
        {
            throw new Win32Exception(401, "Token revoked");
        }
        if (token != null && token.ExpiresAt < DateTime.UtcNow && !token.IsRevoked)
        {
            token.ExpiresAt = DateTime.UtcNow.AddDays(7);
        }

        if (token != null && token.ExpiresAt >= DateTime.UtcNow && !token.IsRevoked)
        {
            return token;
        }
        else
        {
            var refreshToken = _jwt.GenerateRefreshToken();
            var refreshHash = _jwt.HashRefreshToken(refreshToken);
            token = new UserRefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = refreshHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            _dataContext.UserRefreshTokens.Add(token);
        }
        _dataContext.SaveChanges();
        return token;
    }
}