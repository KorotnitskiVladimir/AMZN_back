using AMZN.DTOs.Home;
using AMZN.Repositories.Products;
using AMZN.Shared.Mapping;

namespace AMZN.Services.Home
{

    public class HomeService
    {
        private readonly IProductRepository _products;


        public HomeService(IProductRepository products)
        {
            _products = products;
        }


        public async Task<HomeResponseDto> GetHomeAsync(int take)
        {          
            take = Math.Clamp(take, 1, 100);

            var items = await _products.GetHomeProductsAsync(take);

            var response = new HomeResponseDto();
            response.Products = items.Select(x => x.ToHomeDto()).ToList();

            return response;
        }


    }
}
