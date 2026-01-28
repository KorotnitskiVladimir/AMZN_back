using AMZN.Data.Entities;
using AMZN.Security.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AMZN.Security
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;


        public JwtService(IConfiguration config)
        {
            _config = config;
        }



        public (string token, int expiresInSeconds) GenerateAccessToken(User user)
        {
            // Jwt:Key -> Base64 строка с секретным ключом из конфига (используется для подписи JWT).
            var keyBase64 = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing");

            byte[] keyBytes;
            try
            {
                keyBytes = Convert.FromBase64String(keyBase64);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("Jwt:Key must be a valid Base64 string.", ex);
            }

            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var minutes = int.TryParse(_config["Jwt:ExpiresMinutes"], out var m) ? m : 30;  // если не задано в конфиге - по умолчанию 30 минут
            var expires = now.AddMinutes(minutes);


            // claims юзера:  UserId/Email/Role
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),          // UserId
                new Claim(ClaimTypes.Role, user.Role.ToString()),                  // Role
                new Claim(ClaimTypes.Email, user.Email),                           // Email

                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())  // id токена
            };


            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                notBefore: now,                 // nbf: токен валиден не раньше этого времени (ставим now — валиден сразу)
                expires: expires,               // exp: срок жизни токена
                signingCredentials: creds
            );


            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            var expiresInSeconds = (int)TimeSpan.FromMinutes(minutes).TotalSeconds;

            return (jwt, expiresInSeconds);
        }


        public string GenerateRefreshToken()
        {
            // base64url (без + / =)
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }


        public string HashRefreshToken(string refreshToken)
        {
            // В БД храним только хэш refresh токена (TokenHash)
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
            return Convert.ToHexString(bytes);
        }


    }

}

