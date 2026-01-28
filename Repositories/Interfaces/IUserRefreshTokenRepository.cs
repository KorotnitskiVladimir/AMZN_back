using AMZN.Data.Entities;

namespace AMZN.Repositories.Interfaces
{
    public interface IUserRefreshTokenRepository
    {
        Task AddRefreshTokenAsync(UserRefreshToken token);
        Task<UserRefreshToken?> GetValidByHashAsync(string tokenHash);
        Task RevokeAsync(UserRefreshToken token);


    }
}
