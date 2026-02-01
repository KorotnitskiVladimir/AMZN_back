using AMZN.Data;
using AMZN.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AMZN.Repositories.Users
{
    public class UserRefreshTokenRepository : IUserRefreshTokenRepository
    {
        private readonly DataContext _db;

        public UserRefreshTokenRepository(DataContext db)
        {
            _db = db;
        }


        public async Task AddRefreshTokenAsync(UserRefreshToken token)
        {
            await _db.UserRefreshTokens.AddAsync(token);
            await _db.SaveChangesAsync();
        }

        public async Task<UserRefreshToken?> GetValidByHashAsync(string tokenHash)
        {
            return await _db.UserRefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.TokenHash == tokenHash &&
                    x.IsRevoked == false &&
                    x.ExpiresAt > DateTime.UtcNow);
        }

        public async Task RevokeAsync(UserRefreshToken token)
        {
            token.IsRevoked = true;
            await _db.SaveChangesAsync();
        }
    }
}
