using AMZN.DTOs.Categories;
using AMZN.Repositories.Categories;
using AMZN.Shared.Exceptions;
using AMZN.Shared.Exceptions.Errors;

namespace AMZN.Services.Categories
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categories;

        public CategoryService(ICategoryRepository categories)
        {
            _categories = categories;
        }


        public Task<List<CategoryListItemDto>> GetRootAsync()
        {
            return _categories.GetRootAsync();
        }

        public async Task<List<CategoryListItemDto>> GetByParentAsync(Guid parentId)
        {
            var exists = await _categories.ExistsAsync(parentId);

            if (!exists)
                throw new ApiException(ErrorCodes.CategoryNotFound, "Category not found", StatusCodes.Status404NotFound);

            return await _categories.GetByParentAsync(parentId);
        }

    }

}
