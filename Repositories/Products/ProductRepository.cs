using AMZN.Data;
using AMZN.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AMZN.Repositories.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _db;

        public ProductRepository(DataContext db)
        {
            _db = db;
        }

        public void Add(Product product)
        {
            _db.Products.Add(product);
        }

        public void Update(Product product)
        {
            _db.Products.Update(product);
        }

        public void Remove(Product product)
        {
            _db.Products.Remove(product);
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }

        public Task<List<Product>> GetHomeProductsAsync(int take)
        {
            return _db.Products
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public Task<List<Product>> GetByCategoryAsync(Guid categoryId, int skip, int take)
        {
            return _db.Products
                .AsNoTracking()
                .Where(x => x.CategoryId == categoryId)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public Task<Product?> GetByIdAsync(Guid id)
        {
            return _db.Products
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<Product?> GetByIdWithImagesAsync(Guid id)
        {
            return _db.Products
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == id);
        }


    }
}
