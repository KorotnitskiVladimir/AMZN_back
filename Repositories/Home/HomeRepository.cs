using AMZN.Data;
using AMZN.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AMZN.Repositories.Home
{
    public class HomeRepository : IHomeRepository
    {
        private readonly DataContext _db;

        public HomeRepository(DataContext db)
        {
            _db = db;
        }


        public Task<List<Category>> GetAllCategoriesAsync()
        {
            return _db.Categories
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public Task<List<Product>> GetStockProductsAsync()
        {
            return _db.Products
                .AsNoTracking()
                .Where(x => x.StockQuantity > 0)
                .ToListAsync();
        }

        public Task<List<Product>> GetLastViewedProductsByIdsAsync(List<Guid> productIds)
        {
            if (productIds.Count == 0)
                return Task.FromResult(new List<Product>());

            return _db.Products
                .AsNoTracking()
                .Where(x => productIds.Contains(x.Id) && x.StockQuantity > 0)
                .ToListAsync();
        }
    }
}
