using AMZN.Data;
using AMZN.Data.Entities;

namespace AMZN.Repositories.Orders;

public class OrderRepository : IOrderRepository
{
    private readonly DataContext _dataContext;
    
    public OrderRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task AddOrderAsync(Order order)
    {
        await _dataContext.Orders.AddAsync(order);
        await _dataContext.SaveChangesAsync();
    }
}