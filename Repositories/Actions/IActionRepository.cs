using Action = AMZN.Data.Entities.Action;

namespace AMZN.Repositories.Actions;

public interface IActionRepository
{
    void AddProductAction(Action action);
    
    void AddCategoryAction(Action action);
    
    void ApplyProductAction(Action action);
    
    void ApplyCategoryAction(Action action);
    
    void RemoveProductAction(Action action);
    
    void RemoveCategoryAction(Action action);
    
    bool IsValid(Action action);
    
    List<Action> GetActions();
}