using AMZN.Data.Entities;
using AMZN.Repositories.Brands;


namespace AMZN.Services.BrandService
{
    public class BrandService
    {
        private readonly IBrandRepository _brands;

        public BrandService(IBrandRepository brands)
        {
            _brands = brands;
        }


        public Task<List<Brand>> GetAllAsync()
        {
            return _brands.GetAllAsync();
        }


        public async Task CreateAsync(string name)
        {
            name = name.Trim();

            if (name.Length == 0)
                throw new InvalidOperationException("Brand name is required");
            if (await _brands.ExistsByNameAsync(name))
                throw new InvalidOperationException("Brand already exists");

            var brand = new Brand
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            _brands.Add(brand);
            await _brands.SaveChangesAsync();
        }


        public async Task DeleteAsync(Guid id)
        {
            var brand = await _brands.GetByIdAsync(id);

            if (brand == null) 
                return;
            if (await _brands.IsUsedByAnyProductAsync(id))
                throw new InvalidOperationException("Cannot delete brand that has products");

            _brands.Remove(brand);
            await _brands.SaveChangesAsync();
        }

    }
}
