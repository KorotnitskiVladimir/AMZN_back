using AMZN.Data.Entities;
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
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }


        public async Task<ProductDetailsDto> GetByIdAsync (Guid id)
        {
            var product = await _productRepository.GetDetailsByIdAsync (id);

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

            var total = await _productRepository.CountCatalogProductsAsync(queryParams);
            var items = await _productRepository.GetCatalogProductsAsync(queryParams, skip, q.PageSize);

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
            var brands = await _productRepository.GetCatalogBrandsAsync(categoryId);

            return brands.Select(b => b.ToBrandDto()).ToList();
        }

        public async Task<ProductRatingResponseDto> SetRatingAsync(Guid productId, Guid userId, byte rating)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new ApiException(ErrorCodes.ProductNotFound, "Product not found", StatusCodes.Status404NotFound);

            if (product.SellerId == userId)
                throw new ApiException(ErrorCodes.CannotRateOwnProduct, "You cannot rate your own product", StatusCodes.Status403Forbidden);

            var existingRating = await _productRepository.GetUserRatingAsync(productId, userId);

            var hasChanges = false;

            if (existingRating == null)
            {
                var newRating = new ProductRating
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    UserId = userId,
                    Value = rating
                };

                _productRepository.AddRating(newRating);

                product.RatingSum += rating;
                product.RatingCount += 1;
                hasChanges = true;
            }
            else if (existingRating.Value != rating)
            {
                product.RatingSum = product.RatingSum - existingRating.Value + rating;
                existingRating.Value = rating;
                hasChanges = true;
            }

            if (hasChanges)
            {
                await _productRepository.SaveChangesAsync();
            }

            return new ProductRatingResponseDto
            {
                AverageRating = ProductMapper.CalcRatingHalfStep(product.RatingSum, product.RatingCount),
                RatingsCount = product.RatingCount,
                UserRating = rating
            };
        }

    }
}
