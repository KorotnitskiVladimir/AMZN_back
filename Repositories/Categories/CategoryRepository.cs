using AMZN.Data;
using AMZN.Data.Entities;
using Microsoft.EntityFrameworkCore;

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
    
    public List<Category> GetAll()
    {
        return _dataContext.Categories.ToList();
    }


    public Task<List<Category>> GetAllAsync()
    {
        return _dataContext.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        return _dataContext.Categories
            .AsNoTracking()
            .AnyAsync(x => x.Id == id);
    }

}