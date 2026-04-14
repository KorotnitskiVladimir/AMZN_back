using AMZN.Data.Entities;
using AMZN.Models.Product;
using AMZN.Repositories.Brands;
using AMZN.Repositories.Categories;
using AMZN.Repositories.Products;
using AMZN.Services.Storage.Cloud;
using AMZN.Shared.Transactions;

namespace AMZN.Services.Admin
{
    public class AdminProductService
    {
        private readonly IProductRepository _products;
        private readonly ICategoryRepository _categories;
        private readonly ICloudStorageService _cloud;
        private readonly IBrandRepository _brands;
        private readonly ILogger<AdminProductService> _logger;

        public AdminProductService(
            IProductRepository products,
            ICategoryRepository categories,
            ICloudStorageService cloud,
            IBrandRepository brand,
            ILogger<AdminProductService> logger
            )
        {
            _products = products;
            _categories = categories;
            _cloud = cloud;
            _brands = brand;
            _logger = logger;
        }


        public async Task<ProductCreateViewModel> BuildCreateVmAsync(
            ProductCreateFormModel? form = null, 
            string? errorMessage = null)
        {
            return new ProductCreateViewModel
            {
                Form = form ?? new ProductCreateFormModel(),
                Categories = await _categories.GetAllAsync(),
                Brands = await _brands.GetAllAsync(),
                ErrorMessage = errorMessage
            };
        }


        public async Task<Guid> CreateAsync(ProductCreateFormModel form, Guid sellerId)
        {
            Guid categoryId = form.CategoryId ?? throw new InvalidOperationException("Category is required");
            Guid brandId = form.BrandId ?? throw new InvalidOperationException("Brand is required");

            if (!await _categories.ExistsAsync(categoryId))
                throw new InvalidOperationException("Category does not exist");
            if (await _brands.GetByIdAsync(brandId) == null)
                throw new InvalidOperationException("Brand does not exist");

            decimal currentPrice = decimal.Round(form.CurrentPrice, 2);

            decimal? originalPrice = form.OriginalPrice is > 0
                ? decimal.Round(form.OriginalPrice.Value, 2)
                : null;

            IFormFile primaryFile = form.PrimaryImage ?? throw new InvalidOperationException("Primary image is required");

            string? primaryUrl = null;
            List<string> galleryUrls = new();

            try
            {
                primaryUrl = _cloud.SaveFile(primaryFile);

                // Галерея Продукта (до 10 img)  // sort - их очерёдность
                if (form.Images != null && form.Images.Count > 0)
                {
                    foreach (var file in form.Images)
                    {
                        if (file == null || file.Length == 0) continue;
                        if (galleryUrls.Count >= ProductCreateFormModel.MaxGalleryImages) break;

                        string url = _cloud.SaveFile(file);
                        galleryUrls.Add(url);
                    }
                }

                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    CategoryId = categoryId,
                    BrandId = brandId,
                    SellerId = sellerId,

                    Title = form.Title.Trim(),
                    Description = string.IsNullOrWhiteSpace(form.Description) ? null : form.Description.Trim(),
                    StockQuantity = form.StockQuantity,
                    CurrentPrice = currentPrice,
                    OriginalPrice = originalPrice,
                    PrimaryImageUrl = primaryUrl,
                    CreatedAt = DateTime.UtcNow,
                    RatingSum = 0,
                    RatingCount = 0
                };

                for (int i = 0; i < galleryUrls.Count; i++)
                {
                    product.Images.Add(new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        Url = galleryUrls[i],
                        SortOrder = i
                    });
                }

                _products.Add(product);
                await _products.SaveChangesAsync();

                return product.Id;
            }
            catch
            {
                // не оставляем мусорные картинки в blob
                DeleteBlobFileBestEffort(primaryUrl);
                DeleteBlobFilesBestEffort(galleryUrls);
                throw;
            }
        }


        public async Task<MyProductsListViewModel> BuildMyProductsListVmAsync(Guid sellerId)
        {
            List<Product> sellerProducts = await _products.GetSellerProductsAsync(sellerId);

            return new MyProductsListViewModel
            {
                Products = sellerProducts.Select(product => new MyProductsListItemViewModel
                {
                    Id = product.Id,
                    Title = product.Title,
                    CurrentPrice = product.CurrentPrice,
                    StockQuantity = product.StockQuantity,
                    PrimaryImageUrl = product.PrimaryImageUrl,
                    CreatedAt = product.CreatedAt
                }).ToList()
            };
        }


        public async Task<ProductEditViewModel> BuildProductEditVmAsync(
            Guid productId,
            Guid sellerId,
            ProductEditFormModel? form = null,
            string? errorMessage = null
            )
        {
            Product product = await _products.GetByIdWithImagesAsync(productId)
                ?? throw new InvalidOperationException("Product not found");

            if (product.SellerId != sellerId)
                throw new InvalidOperationException("Product not found");

            List<ProductImage> orderedExistingImages = product.Images
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Id)
                .ToList();

            ProductEditFormModel editForm = form ?? new ProductEditFormModel
            {
                Title = product.Title,
                Description = product.Description,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                BrandId = product.BrandId,
                CurrentPrice = product.CurrentPrice,
                OriginalPrice = product.OriginalPrice,
                ExistingGalleryImageIdsInOrder = orderedExistingImages
                    .Select(x => x.Id)
                    .ToList()
            };

            List<ExistingProductImageViewModel> existingGalleryImages =
                BuildExistingGalleryImagesForEdit(orderedExistingImages, editForm.ExistingGalleryImageIdsInOrder);

            return new ProductEditViewModel
            {
                ProductId = product.Id,
                Form = editForm,
                CurrentPrimaryImageUrl = product.PrimaryImageUrl,
                ExistingGalleryImages = existingGalleryImages,
                Categories = await _categories.GetAllAsync(),
                Brands = await _brands.GetAllAsync(),
                ErrorMessage = errorMessage
            };
        }


        public async Task UpdateAsync(Guid productId, ProductEditFormModel form, Guid sellerId)
        {
            Product product = await _products.GetByIdWithImagesAsync(productId)
                ?? throw new InvalidOperationException("Product not found");

            if (product.SellerId != sellerId)
                throw new InvalidOperationException("Product not found");

            Guid categoryId = form.CategoryId ?? throw new InvalidOperationException("Category is required");
            Guid brandId = form.BrandId ?? throw new InvalidOperationException("Brand is required");

            if (!await _categories.ExistsAsync(categoryId))
                throw new InvalidOperationException("Category does not exist");
            if (await _brands.GetByIdAsync(brandId) == null)
                throw new InvalidOperationException("Brand does not exist");

            List<ProductImage> currentGalleryImages = product.Images
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Id)
                .ToList();

            Dictionary<Guid, ProductImage> currentGalleryImagesById = currentGalleryImages
                .ToDictionary(x => x.Id);

            List<Guid> existingGalleryImageIdsInOrder = form.ExistingGalleryImageIdsInOrder ?? new();

            if (existingGalleryImageIdsInOrder.Count != existingGalleryImageIdsInOrder.Distinct().Count())
                throw new InvalidOperationException("Duplicate gallery image ids");

            foreach (Guid imageId in existingGalleryImageIdsInOrder)
            {
                if (!currentGalleryImagesById.ContainsKey(imageId))
                    throw new InvalidOperationException("Invalid gallery image ids");
            }

            // Существующие картинки, которые остаются после delete / reorder
            List<ProductImage> remainingExistingImages = existingGalleryImageIdsInOrder
                .Select(imageId => currentGalleryImagesById[imageId])
                .ToList();

            // Существующие картинки, которые юзер убрал из текущей gallery
            List<ProductImage> deletedExistingImages = currentGalleryImages
                .Where(x => !existingGalleryImageIdsInOrder.Contains(x.Id))
                .ToList();

            // Если текущая gallery не менялась, reorder существующих img не нужен
            List<Guid> currentGalleryImageIdsInOrder = currentGalleryImages
                .Select(x => x.Id)
                .ToList();

            bool existingGalleryChanged = !currentGalleryImageIdsInOrder.SequenceEqual(existingGalleryImageIdsInOrder);

            // Новые картинки, которые добавим в конец gallery
            List<IFormFile> newGalleryImageFiles = form.NewGalleryImages?
                .Where(x => x != null && x.Length > 0)
                .ToList()
                ?? new List<IFormFile>();

            int finalGalleryImageCount = remainingExistingImages.Count + newGalleryImageFiles.Count;
            if (finalGalleryImageCount > ProductEditFormModel.MaxGalleryImages)
                throw new InvalidOperationException("Too many gallery images (max 10)");

            product.Title = form.Title.Trim();
            product.Description = string.IsNullOrWhiteSpace(form.Description) ? null : form.Description.Trim();
            product.StockQuantity = form.StockQuantity;
            product.CategoryId = categoryId;
            product.BrandId = brandId;
            product.CurrentPrice = decimal.Round(form.CurrentPrice, 2);
            product.OriginalPrice = form.OriginalPrice is > 0
                ? decimal.Round(form.OriginalPrice.Value, 2)
                : null;

            string? oldPrimaryImageUrl = null;
            string? newPrimaryImageUrl = null;
            List<string> newGalleryImageUrls = new();
            List<string> deletedExistingImageUrls = deletedExistingImages
                .Select(x => x.Url)
                .ToList();

            try
            {
                if (form.NewPrimaryImage != null)
                {
                    newPrimaryImageUrl = _cloud.SaveFile(form.NewPrimaryImage);
                    oldPrimaryImageUrl = product.PrimaryImageUrl;
                    product.PrimaryImageUrl = newPrimaryImageUrl;
                }

                foreach (IFormFile file in newGalleryImageFiles)
                {
                    string newGalleryImageUrl = _cloud.SaveFile(file);
                    newGalleryImageUrls.Add(newGalleryImageUrl);
                }

                int sortOrderBase = currentGalleryImages.Count == 0
                    ? 0
                    : currentGalleryImages.Max(x => x.SortOrder) + 1;

                if (deletedExistingImages.Count > 0)
                    _products.RemoveProductImages(deletedExistingImages);

                if (existingGalleryChanged)
                {
                    // Если порядок existing gallery менялся,
                    // уводим оставшиеся картинки в новый диапазон SortOrder выше текущего.
                    for (int i = 0; i < remainingExistingImages.Count; i++)
                        remainingExistingImages[i].SortOrder = sortOrderBase + i;
                }

                int nextSortOrder = existingGalleryChanged
                    ? sortOrderBase + remainingExistingImages.Count
                    : remainingExistingImages.Count == 0
                        ? 0
                        : remainingExistingImages.Max(x => x.SortOrder) + 1;

                List<ProductImage> newProductImages = new();

                foreach (string galleryImageUrl in newGalleryImageUrls)
                {
                    newProductImages.Add(new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        Url = galleryImageUrl,
                        SortOrder = nextSortOrder++
                    });
                }

                if (newProductImages.Count > 0)
                    _products.AddProductImages(newProductImages);

                await _products.SaveChangesAsync();
            }
            catch
            {
                DeleteBlobFileBestEffort(newPrimaryImageUrl);
                DeleteBlobFilesBestEffort(newGalleryImageUrls);
                throw;
            }

            DeleteBlobFileBestEffort(oldPrimaryImageUrl);
            DeleteBlobFilesBestEffort(deletedExistingImageUrls);
        }


        public async Task DeleteAsync(Guid productId, Guid sellerId)
        {
            Product product = await _products.GetByIdWithImagesAsync(productId)
                ?? throw new InvalidOperationException("Product not found");

            if (product.SellerId != sellerId)
                throw new InvalidOperationException("Product not found");

            List<string> blobUrlsToDelete = new() { product.PrimaryImageUrl };
            blobUrlsToDelete.AddRange(product.Images.Select(x => x.Url));

            _products.Remove(product);
            await _products.SaveChangesAsync();

            DeleteBlobFilesBestEffort(blobUrlsToDelete);
        }


        private static List<ExistingProductImageViewModel> BuildExistingGalleryImagesForEdit(
            List<ProductImage> currentGalleryImages,
            List<Guid> existingGalleryImageIdsInOrder
            )
        {
            if (currentGalleryImages.Count == 0 || existingGalleryImageIdsInOrder.Count == 0)
                return new List<ExistingProductImageViewModel>();

            Dictionary<Guid, ProductImage> currentGalleryImagesById = currentGalleryImages
                .ToDictionary(x => x.Id);

            List<ExistingProductImageViewModel> result = new();

            foreach (Guid imageId in existingGalleryImageIdsInOrder)
            {
                if (!currentGalleryImagesById.TryGetValue(imageId, out ProductImage? image))
                    continue;

                result.Add(new ExistingProductImageViewModel
                {
                    Id = image.Id,
                    Url = image.Url,
                    SortOrder = image.SortOrder
                });
            }

            return result;
        }


        private void DeleteBlobFileBestEffort(string? fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return;

            try
            {
                _cloud.DeleteByUrl(fileUrl);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete blob file: {FileUrl}", fileUrl);
            }
        }


        private void DeleteBlobFilesBestEffort(IEnumerable<string> fileUrls)
        {
            foreach (string fileUrl in fileUrls.Distinct())
                DeleteBlobFileBestEffort(fileUrl);
        }
    }
}