using AMZN.Data.Entities;

namespace AMZN.Repositories.Users;

public interface IDeletedUserRepository
{
    Task AddAsync(DeletedUser deletedUser);
}