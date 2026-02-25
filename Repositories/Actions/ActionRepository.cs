using AMZN.Data;
using Action = AMZN.Data.Entities.Action;

namespace AMZN.Repositories.Actions;

public class ActionRepository : IActionRepository
{
    private readonly DataContext _dataContext;

    public ActionRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public void AddAction(Action action)
    {
        _dataContext.Actions.Add(action);
        _dataContext.SaveChanges();
    }
    
    public void ApplyAction(Action action)
    {
        var product = _dataContext.Products.FirstOrDefault(p => p.Id == action.ProductId);
        if (product != null)
        {
            product.OriginalPrice = product.CurrentPrice;
            product.CurrentPrice *= (decimal)((100 - action.Amount) * 0.01);
            _dataContext.SaveChanges();
        }
    }

    public void RemoveAction(Action action)
    {
        var product = _dataContext.Products.FirstOrDefault(p => p.Id == action.ProductId);
        if (product != null)
        {
            if (product.OriginalPrice != null)
            {
                product.CurrentPrice = (decimal)product.OriginalPrice;
                product.OriginalPrice = null;
                _dataContext.SaveChanges();
            }
        }
    }
}