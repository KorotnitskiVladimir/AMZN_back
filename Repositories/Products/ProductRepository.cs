using AMZN.Data;
using AMZN.Data.Entities;
using AMZN.Repositories.Products.Queries;
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

        public Task<Product?> GetDetailsByIdAsync(Guid id)
        {
            return _db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // catalog
        // кол-во Products подходящих под фильтры запроса
        public Task<int> CountCatalogProductsAsync(ProductListQueryParams queryParams)
        {
            return BuildCatalogQuery(queryParams).CountAsync();
        }

        public Task<List<Product>> GetCatalogProductsAsync(ProductListQueryParams queryParams, int skip, int take)
        {
            var query = BuildCatalogQuery(queryParams);
            query = ApplySort(query, queryParams.Sort);

            return query
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }


        public Task<List<Brand>> GetCatalogBrandsAsync(Guid? categoryId)
        {
            var productQuery = _db.Products.AsNoTracking();

            if (categoryId != null)
                productQuery = productQuery.Where(p => p.CategoryId == categoryId.Value);

            var brandIdsQuery = productQuery
                .Select(p => p.BrandId)
                .Distinct();

            return _db.Brands
                .AsNoTracking()
                .Where(b => brandIdsQuery.Contains(b.Id))
                .OrderBy(b => b.Name)
                .ToListAsync();
        }


        private IQueryable<Product> BuildCatalogQuery(ProductListQueryParams q)
        {
            var query = _db.Products.AsNoTracking();
            
            if (q.CategoryId != null)
                query = query.Where(p => p.CategoryId == q.CategoryId.Value);

            if (q.BrandIds.Count > 0)
                query = query.Where(p => q.BrandIds.Contains(p.BrandId));

            if(q.MinPrice  != null)
                query = query.Where(p => p.CurrentPrice >= q.MinPrice.Value);

            if (q.MaxPrice != null)
                query = query.Where(p => p.CurrentPrice <= q.MaxPrice.Value);

            if (q.MinRating != null && q.MinRating >0)
            {
                var minRating = q.MinRating.Value;
                query = query.Where(p =>
                    p.RatingCount > 0 &&
                    ((decimal)p.RatingSum / p.RatingCount) >= minRating);
            }

            return query;
        }


        private static IQueryable<Product> ApplySort(IQueryable<Product> query, string? sort)
        {
            if (string.IsNullOrWhiteSpace(sort))
                sort = "featured";

            sort = sort.Trim().ToLowerInvariant();

            return sort switch
            {
                "featured" => query
                    .OrderByDescending(p => p.RatingCount)
                    .ThenByDescending(p => p.CreatedAt),

                "price_asc" => query.OrderBy( p => p.CurrentPrice),
                "price_desc" => query.OrderByDescending(p => p.CurrentPrice),

                "rating" or "rating_desc" => query
                        .OrderByDescending(p => p.RatingCount > 0 
                            ? ((decimal)p.RatingSum / p.RatingCount) 
                            : 0m)
                        .ThenByDescending(p => p.RatingCount),

                "newest" => query.OrderByDescending(p => p.CreatedAt),

                _ => query.OrderByDescending(p => p.RatingCount).ThenByDescending(p => p.CreatedAt)
            };
        }


    }
}
