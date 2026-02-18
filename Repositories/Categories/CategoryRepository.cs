using AMZN.Data;
using AMZN.Data.Entities;

namespace AMZN.Repositories.Categories;

public class CategoryRepository : ICategoryRepository
{
    private readonly DataContext _dataContext;

    public CategoryRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public Category? GetById(string id)
    {
        return _dataContext.Categories.FirstOrDefault(x => x.Id.ToString() == id);
    }
    
    public void AddCategory(Category category)
    {
        _dataContext.Categories.Add(category);
        _dataContext.SaveChanges();
    }
}