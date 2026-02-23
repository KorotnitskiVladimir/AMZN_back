using AMZN.Data.Entities;

namespace AMZN.Repositories.Categories;

public interface ICategoryRepository
{
    Category? GetById(string id);
    void AddCategory(Category category);

    List<Category> GetAll();

    Task<List<Category>> GetAllAsync();
    Task<bool> ExistsAsync(Guid id);
}