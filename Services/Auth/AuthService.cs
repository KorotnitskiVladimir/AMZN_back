using AMZN.Data.Entities;
using AMZN.DTOs.Auth;
using AMZN.Repositories.Users;
using AMZN.Security.Passwords;
using AMZN.Security.Tokens;

namespace AMZN.Services.Auth
{
    public class AuthService : IAuthService
    {

        private const int RefreshDaysDefault = 7;
        private const int RefreshDaysMin = 1;
        private const int RefreshDaysMax = 365;


        private readonly IUserRepository _users;
        private readonly IUserRefreshTokenRepository _refreshTokens;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwt;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;


        public AuthService(
            IUserRepository users,
            IUserRefreshTokenRepository refreshTokens,
            IPasswordHasher passwordHasher,
            IJwtService jwt,
            IConfiguration config,
            ILogger<AuthService> logger)
        {
            _users = users;
            _refreshTokens = refreshTokens;
            _passwordHasher = passwordHasher;
            _jwt = jwt;
            _config = config;
            _logger = logger;
        }



        public Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponseDto> RefreshAsync(RefreshRequestDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
