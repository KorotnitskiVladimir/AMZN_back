using AMZN.Data.Entities;
using AMZN.DTOs.Home;

namespace AMZN.Repositories.Home
{
    public interface IHomeRepository
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<CategoryProductCountsDto>> GetCategoryProductCountsAsync(decimal minRating);

        Task<List<Product>> GetTopRatedProductsForCategoriesAsync(List<Guid> categoryIds, decimal minRating, int take);

        Task<List<Product>> GetTopRatedProductsAsync(decimal minRating, int take);

        Task<List<Product>> GetTopRatedProductsUnderPriceAsync(decimal maxPrice, decimal minRating, int take);

        Task<List<Product>> GetProductsByIdsAsync(List<Guid> productIds);
    }
}
