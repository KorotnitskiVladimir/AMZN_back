using AMZN.Data;

namespace AMZN.Shared.Transactions
{
    public class EfTransactionManager : ITransactionManager
    {
        private readonly DataContext _db;

        public EfTransactionManager(DataContext db)
        {
            _db = db;
        }


        public async Task ExecuteAsync(Func<Task> action)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                await action();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var result = await action();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}