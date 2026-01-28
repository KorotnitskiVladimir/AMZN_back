using AMZN.Data.Entities;

namespace AMZN.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<bool> IsEmailTakenAsync(string email);
        Task<User?> GetByEmailAsync(string email);
        


    }
}
