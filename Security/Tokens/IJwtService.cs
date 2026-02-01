using AMZN.Data.Entities;

namespace AMZN.Security.Tokens
{
    public interface IJwtService
    {
        (string token, int expiresInSeconds) GenerateAccessToken(User user);
        string GenerateRefreshToken();
        string HashRefreshToken(string refreshToken);

    }
}
