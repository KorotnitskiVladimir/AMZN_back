using AMZN.Data.Entities;
using AMZN.Models;
using AMZN.Models.Category;
using AMZN.Repositories.Categories;
using AMZN.Services.Storage.Cloud;
using AMZN.Services.Storage.Local;

namespace AMZN.Services.Admin;

public class AdminCategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILocalsStorageService _localStorageService;
    private readonly FormsValidators _formsValidator;
    private readonly ICloudStorageService _cloudStorageService;
    
    public AdminCategoryService(ICategoryRepository categoryRepository, 
        ILocalsStorageService localStorageService, 
        FormsValidators formsValidator, 
        ICloudStorageService cloudStorageService)
    {
        _categoryRepository = categoryRepository;
        _localStorageService = localStorageService;
        _formsValidator = formsValidator;
        _cloudStorageService = cloudStorageService;
    }
    
    public (bool, object) AddCategory(CategoryFormModel formModel)
    {
        Dictionary<string, string> errors = _formsValidator.ValidateCategory(formModel);
        if (errors.Count == 0)
        {
            Guid id = Guid.NewGuid();
            var parent = _categoryRepository.GetById(formModel.ParentCategory);
            Guid? parentId = null;
            if (parent != null)
            {
                parentId = parent.Id;
            }

            Category category = new()
            {
                Id = id,
                Name = formModel.Name,
                Description = formModel.Description,
                //ImageUrl = _localsStorageService.SaveFile(model.Image),
                ImageUrl = _cloudStorageService.SaveFile(formModel.Image),
                CreatedAt = DateTime.UtcNow,
            };
            if (parent != null)
            {
                category.ParentId = parentId;
                category.ParentCategory = parent;
            }

            _categoryRepository.AddCategory(category);
            return (true, "Category added successfully");
        }
        return (false, errors.Values);
    }
    
    public List<Category> GetAll()
    {
        return _categoryRepository.GetAll();
    }
}