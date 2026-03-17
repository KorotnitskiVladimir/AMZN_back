using AMZN.Data;
using AMZN.Data.Entities;
using AMZN.DTOs.Categories;
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
    
    public Category? GetByName(string name)
    {
        return _dataContext.Categories.FirstOrDefault(x => x.Name == name);
    }

    public Task<List<CategoryListItemDto>> GetRootAsync()
    {
        return _dataContext.Categories
            .AsNoTracking()
            .Where(x => x.ParentId == null)
            .OrderBy(x => x.Name)
            .Select(x => new CategoryListItemDto
            {
                Id = x.Id,
                ParentId = x.ParentId,
                Name = x.Name,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                HasChildren = _dataContext.Categories.Any(c => c.ParentId == x.Id)
            })
            .ToListAsync();
    }

    public Task<List<CategoryListItemDto>> GetByParentAsync(Guid parentId)
    {
        return _dataContext.Categories
            .AsNoTracking()
            .Where(x => x.ParentId == parentId)
            .OrderBy(x => x.Name)
            .Select(x => new CategoryListItemDto
            {
                Id = x.Id,
                ParentId = x.ParentId,
                Name = x.Name,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                HasChildren = _dataContext.Categories.Any(c => c.ParentId == x.Id)
            })
            .ToListAsync();
    }

    public Task SaveChangesAsync()
    {
        return _dataContext.SaveChangesAsync();
    }

}