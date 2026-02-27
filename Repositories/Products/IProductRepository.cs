using AMZN.Data.Entities;

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
    }

}
