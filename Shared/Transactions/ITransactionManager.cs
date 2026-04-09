namespace AMZN.Shared.Transactions
{
    public interface ITransactionManager
    {
        Task ExecuteAsync(Func<Task> action);
        Task<T> ExecuteAsync<T>(Func<Task<T>> action);
    }
}
