using AMZN.Data.Entities;

namespace AMZN.Repositories.Home
{
    public interface IHomeRepository
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<Product>> GetStockProductsAsync();
        Task<List<Product>> GetLastViewedProductsByIdsAsync(List<Guid> productIds);
    }
}
