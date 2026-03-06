using AMZN.Data;
using AMZN.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AMZN.Repositories.Brands
{
    public class BrandRepository : IBrandRepository
    {
        private readonly DataContext _db;
        public BrandRepository(DataContext db ) 
        {
            _db = db;
        }


        public Task<List<Brand>> GetAllAsync()
        {
            return _db.Brands
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public Task<Brand?> GetByIdAsync(Guid id)
        {
            return _db.Brands.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<bool> ExistsByNameAsync(string name)
        {
            return _db.Brands.AnyAsync(x => x.Name == name);
        }

        public void Add(Brand brand)
        {
            _db.Brands.Add(brand);
        }

        public void Remove(Brand brand)
        {
            _db.Brands.Remove(brand);
        }

        public Task<bool> IsUsedByAnyProductAsync(Guid brandId)
        {
            return _db.Products.AnyAsync(p => p.BrandId == brandId);
        }

        public Task<int> SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }

    }


}
