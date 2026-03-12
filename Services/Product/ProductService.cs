using AMZN.DTOs.Brands;
using AMZN.DTOs.Common;
using AMZN.DTOs.Products;
using AMZN.Repositories.Products;
using AMZN.Repositories.Products.Queries;
using AMZN.Shared.Exceptions;
using AMZN.Shared.Exceptions.Errors;
using AMZN.Shared.Mapping;

namespace AMZN.Services.Products
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


        public async Task<PagedResult<ProductCardDto>> GetCatalogPageAsync(ProductListQueryDto q)
        {
            var skip = (q.Page - 1) * q.PageSize;

            var queryParams = new ProductListQueryParams
            {
                CategoryId = q.CategoryId,
                BrandIds = q.BrandIds.ToList(),
                MinPrice = q.MinPrice,
                MaxPrice = q.MaxPrice,
                MinRating = q.MinRating,
                Sort = q.Sort
            };

            var total = await _products.CountCatalogProductsAsync(queryParams);
            var items = await _products.GetCatalogProductsAsync(queryParams, skip, q.PageSize);

            return new PagedResult<ProductCardDto>
            {
                Items = items.Select(x => x.ToCardDto()).ToList(),
                Page = q.Page,
                PageSize = q.PageSize,
                TotalItems = total
            };
        }


        public async Task<List<BrandDto>> GetCatalogBrandsAsync(Guid? categoryId)
        {
            var brands = await _products.GetCatalogBrandsAsync(categoryId);

            return brands.Select(b => b.ToBrandDto()).ToList();
        }


    }
}
