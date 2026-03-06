using AMZN.Data.Entities;

namespace AMZN.Repositories.Brands
{
    public interface IBrandRepository
    {
        Task<List<Brand>> GetAllAsync();
        Task<Brand?> GetByIdAsync(Guid id);
        Task<bool> ExistsByNameAsync(string name);
        void Add(Brand brand);
        void Remove(Brand brand);
        Task<bool> IsUsedByAnyProductAsync(Guid brandId);
        Task<int> SaveChangesAsync();

    }

}
