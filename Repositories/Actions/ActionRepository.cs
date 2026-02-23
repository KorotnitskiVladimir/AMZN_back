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
}