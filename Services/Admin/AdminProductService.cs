using AMZN.Data.Entities;
using AMZN.Models.Product;
using AMZN.Repositories.Categories;
using AMZN.Repositories.Products;
using AMZN.Services.Storage.Cloud;

namespace AMZN.Services.Admin
{
    public class AdminProductService
    {
        private readonly IProductRepository _products;
        private readonly ICategoryRepository _categories;
        private readonly ICloudStorageService _cloud;

        public AdminProductService(
            IProductRepository products,
            ICategoryRepository categories,
            ICloudStorageService cloud)
        {
            _products = products;
            _categories = categories;
            _cloud = cloud;
        }


        public async Task<ProductCreateViewModel> BuildCreateVmAsync(ProductCreateFormModel? form = null, string? error = null)
        {
            return new ProductCreateViewModel
            {
                Form = form ?? new ProductCreateFormModel(),
                Categories = await _categories.GetAllAsync(),
                ErrorMessage = error
            };
        }


        public async Task CreateAsync(ProductCreateFormModel form)
        {
            Guid categoryId = form.CategoryId ?? throw new InvalidOperationException("CategoryId is required");

            if (!await _categories.ExistsAsync(categoryId))
                throw new InvalidOperationException("Category does not exist");

            decimal currentPrice = decimal.Round(form.CurrentPrice, 2);

            decimal? originalPrice = form.OriginalPrice is > 0
                ? decimal.Round(form.OriginalPrice.Value, 2)
                : null;

            IFormFile primaryFile = form.PrimaryImage ?? throw new InvalidOperationException("Primary image is required");
            string primaryUrl = _cloud.SaveFile(primaryFile);

            var product = new Data.Entities.Product
            {
                Id = Guid.NewGuid(),
                CategoryId = categoryId,
                Title   = form.Title.Trim(),
                Description   = string.IsNullOrWhiteSpace(form.Description) ? null : form.Description.Trim(),
                CurrentPrice  = currentPrice,
                OriginalPrice = originalPrice,
                PrimaryImageUrl = primaryUrl,
                CreatedAt = DateTime.UtcNow,
                RatingSum = 0,
                RatingCount = 0
            };

            // Галерея Продукта (до 10 img)  // sort - их очерёдность
            if (form.Images != null && form.Images.Count > 0)
            {
                int sort = 0;

                foreach (var file in form.Images)
                {
                    if (file == null || file.Length == 0) continue;
                    if (sort >= ProductCreateFormModel.MaxGalleryImages) break;

                    string url = _cloud.SaveFile(file);

                    product.Images.Add(new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        Url = url,
                        SortOrder = sort++
                    });
                }
            }

            _products.Add(product);
            await _products.SaveChangesAsync();
        }


    }
}