using AMZN.Data.Entities;

namespace AMZN.Repositories.Orders;

public interface IOrderRepository
{
    Task AddOrderAsync(Order order);
    Task AddOrderItemAsync(OrderItem orderItem);
    Task<Order?> GetPendingOrderByUserIdAsync(Guid userId);
    Task UpdateOrderAsync(Order order);
    Task RemoveOrderItemsAsync(Order order);
}