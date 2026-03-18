using AMZN.Data;
using AMZN.Data.Entities;
using AMZN.Repositories.Users;
using Microsoft.EntityFrameworkCore;

namespace AMZN.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _db;

        public UserRepository(DataContext db)
        {
            _db = db;
        }


        public async Task<bool> IsEmailTakenAsync(string email)
        {
            return await _db.Users.AsNoTracking().AnyAsync(x => x.Email == email);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.Id.ToString() == id);
        }

        public async Task UpdateUserAsync(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

    }
}
