using AMZN.Data;
using AMZN.Data.Entities;

namespace AMZN.Repositories.Users;

public class DeletedUserRepository : IDeletedUserRepository
{
    private readonly DataContext _dataContext;

    public DeletedUserRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    public async Task AddAsync(DeletedUser deletedUser)
    {
        _dataContext.DeletedUsers.Add(deletedUser);
        await _dataContext.SaveChangesAsync();
    }
}