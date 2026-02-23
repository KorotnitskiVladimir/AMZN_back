using Action = AMZN.Data.Entities.Action;

namespace AMZN.Repositories.Actions;

public interface IActionRepository
{
    void AddAction(Action action);
}