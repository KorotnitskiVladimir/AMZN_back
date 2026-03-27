using AMZN.Data.Entities;

namespace AMZN.Repositories.Search
{
    public interface ISearchRepository
    {
        Task<List<Product>> GetProductSuggestionsAsync(string query, int take);
        Task<List<Category>> GetCategorySuggestionsAsync(string query, int take);
        Task<List<Brand>> GetBrandSuggestionsAsync(string query, int take);
    }
}
