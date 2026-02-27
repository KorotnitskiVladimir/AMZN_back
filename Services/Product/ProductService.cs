using AMZN.DTOs.Products;
using AMZN.Repositories.Products;
using AMZN.Shared.Exceptions;
using AMZN.Shared.Exceptions.Errors;
using AMZN.Shared.Mapping;

namespace AMZN.Services.Product
{
    public class ProductService
    {
        private readonly IProductRepository _products;

        public ProductService(IProductRepository products)
        {
            _products = products;
        }

        public async Task<ProductDetailsDto> GetByIdAsync (Guid id)
        {
            var product = await _products.GetDetailsByIdAsync (id);

            if (product == null)
                throw new ApiException(ErrorCodes.ProductNotFound, "Product not found", StatusCodes.Status404NotFound);

            return product.ToDetailsDto();
        }
    }
}
