using AMZN.Data.Entities;

namespace AMZN.Repositories.Orders;

public interface IOrderRepository
{
    Task AddOrderAsync(Order order);
}