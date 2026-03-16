using AMZN.Data.Entities;
using AMZN.DTOs.Products;
using AMZN.Repositories.Products.Queries;

namespace AMZN.Repositories.Products
{

    public interface IProductRepository
    {
        void Add(Product product);
        void Update(Product product);
        void Remove(Product product);
        Task SaveChangesAsync();

        Task<List<Product>> GetHomeProductsAsync(int take);
        Task<List<Product>> GetByCategoryAsync(Guid categoryId, int skip, int take);

        Task<Product?> GetByIdAsync(Guid id);
        Task<Product?> GetByIdWithImagesAsync(Guid id);
        Task<Product?> GetDetailsByIdAsync(Guid id);
      

        // Catalog Page
        Task<int> CountCatalogProductsAsync(ProductListQueryParams queryParams);
        Task<List<Product>> GetCatalogProductsAsync(ProductListQueryParams queryParams, int skip, int take);
        Task<List<Brand>> GetCatalogBrandsAsync(Guid? categoryId);

        // Product Rating
        Task<ProductRating?> GetUserRatingAsync(Guid productId, Guid userId);
        void AddRating(ProductRating rating);

    }

}
