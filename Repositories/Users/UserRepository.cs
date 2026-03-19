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

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task AddPaymentMethodAsync(PaymentMethod paymentMethod, User user)
        {
            await _db.PaymentMethods.AddAsync(paymentMethod);
            user.PaymentMethods.Add(paymentMethod);
            await _db.SaveChangesAsync();
        }
        
        public async Task<List<PaymentMethod>> GetUserPaymentMethodsAsync(User user)
        {
            return await _db.PaymentMethods.Where(x => x.UserId == user.Id).ToListAsync();
        }
        
        public async Task DeletePaymentMethodAsync(PaymentMethod paymentMethod, User user)
        {
            _db.PaymentMethods.Remove(paymentMethod);
            user.PaymentMethods.Remove(paymentMethod);
            await _db.SaveChangesAsync();
        }

        public void SetDefaultPaymentMethod(PaymentMethod paymentMethod, User user)
        {
            paymentMethod.IsDefault = true;
            foreach (var pm in user.PaymentMethods)
            {
                if (pm.IsDefault && pm.Id != paymentMethod.Id)
                { 
                    pm.IsDefault = false;
                }
            }
        }

        public async Task<PaymentMethod?> GetDefaultPaymentMethodAsync(User user)
        {
            return await _db.PaymentMethods.FirstOrDefaultAsync(x => x.UserId == user.Id && x.IsDefault);
        }
    }
}
