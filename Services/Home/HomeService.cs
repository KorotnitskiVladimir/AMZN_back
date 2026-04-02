using AMZN.Data.Entities;
using AMZN.DTOs.Home;
using AMZN.DTOs.Products;
using AMZN.Repositories.Home;
using AMZN.Shared.Mapping;

namespace AMZN.Services.Home
{
    public class HomeService
    {
        private const decimal HomeMinRating = 2.5m;
        private const decimal MoreProductsMinRating = HomeMinRating;

        private const decimal UnderTwentyMaxPrice = 20m;

        private const int TopCategoriesCount = 3;           // Топ 3 больш. карточки
        private const int CategoryBlocksCount = 4;          // 4 блока с 2х2 продуктс
        private const int CategoryBlockProductsCount = 4;   // 2х2 products
        private const int PopularProductsCount = 3;         // popular products
        private const int PopularCategoriesCount = 3;       // popular categories
        private const int ProductsUnderTwentyCount = 10;
        private const int MoreProductsCount = 7;

        private readonly IHomeRepository _homeRepository;

        public HomeService(IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
        }


        public async Task<HomeResponseDto> GetHomeAsync()
        {
            List<Category> categories = await _homeRepository.GetAllCategoriesAsync();
            List<Product>  inStockProducts = await _homeRepository.GetStockProductsAsync();

            List<Product> homeProducts = inStockProducts.Where(x => HasMinRating(x, HomeMinRating)).ToList();

            // TODO: More Products -> сделать рандом и убрать дубли?
            List<Product> moreProductsSource = inStockProducts.Where(x => HasMinRating(x, MoreProductsMinRating)).ToList();

            // ParentId -> список прямых подкатегорий
            Dictionary<Guid, List<Category>> subcategoriesByParentId = new();

            foreach (Category category in categories)
            {
                if (category.ParentId == null)
                    continue;

                Guid parentId = category.ParentId.Value;

                if (!subcategoriesByParentId.ContainsKey(parentId))
                    subcategoriesByParentId[parentId] = new List<Category>();

                subcategoriesByParentId[parentId].Add(category);
            }

            // продукты по категориям, для сборки блоков home page
            Dictionary<Guid, List<Product>> productsByCategory = homeProducts
                .GroupBy(x => x.CategoryId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Топ 3 большие карточки (root category)
            List<HomeCategoryCardDto> topCategories = BuildTopCategories(categories, subcategoriesByParentId, productsByCategory);

            // Id чтобы не дублировать топ3 карточки в 2х2 блоках
            List<Guid> topCategoryIds = topCategories.Select(x => x.Id).ToList();


            // Блоки 2х2 subcategory
            List<HomeCategoryBlockDto> categoryBlocks = BuildCategoryBlocks(categories, subcategoriesByParentId, productsByCategory, topCategoryIds);

            // PopularProducts
            List<ProductCardDto> popularProducts = BuildProductCards(homeProducts, PopularProductsCount);

            // PopularCategories  (исключает топ3 карточки) 
            List<HomeCategoryCardDto> popularCategories = BuildPopularCategories(
                categories,
                subcategoriesByParentId,
                productsByCategory,
                topCategoryIds
                );


            // Товары до $20
            List<ProductCardDto> productsUnderTwenty = BuildProductCards(
                homeProducts.Where(x => x.CurrentPrice <= UnderTwentyMaxPrice),
                ProductsUnderTwentyCount
                );

            // More Products
            List<ProductCardDto> moreProducts = BuildProductCards(moreProductsSource, MoreProductsCount);


            return new HomeResponseDto
            {
                TopCategories = topCategories,
                CategoryBlocks = categoryBlocks,
                PopularProducts = popularProducts,
                PopularCategories = popularCategories,
                ProductsUnderTwenty = productsUnderTwenty,
                MoreProducts = moreProducts
            };
        }

        public async Task<List<ProductCardDto>> GetLastViewedAsync(List<Guid> productIds)
        {
            if (productIds.Count == 0)
                return new List<ProductCardDto>();

            List<Product> products = await _homeRepository.GetLastViewedProductsByIdsAsync(productIds);
            Dictionary<Guid, Product> productsById = products.ToDictionary(x => x.Id);
            var result = new List<ProductCardDto>();

            foreach (Guid productId in productIds)
            {
                if (productsById.TryGetValue(productId, out var product))
                    result.Add(product.ToCardDto());
            }

            return result;
        }


        // # Helpers

        // Топ 3 большие карточки макета
        private static List<HomeCategoryCardDto> BuildTopCategories(
            List<Category> categories,
            Dictionary<Guid, List<Category>> subcategoriesByParentId,
            Dictionary<Guid, List<Product>> productsByCategory
            )
        {
            return categories
                .Where(x => x.ParentId == null)
                .Select(x => new
                {
                    Category = x,

                    // Для top category считаем товары по всей ветке,
                    // сама категория + все подкатегории ниже
                    Products = GetProductsInCategoryTree(x.Id, subcategoriesByParentId, productsByCategory)
                })
                .Where(x => x.Products.Count > 0)
                .OrderByDescending(x => x.Products.Count)
                .ThenByDescending(x => x.Products.Sum(p => p.RatingCount))
                .ThenBy(x => x.Category.Name)
                .Take(TopCategoriesCount)
                .Select(x => new HomeCategoryCardDto
                {
                    Id = x.Category.Id,
                    Name = x.Category.Name,
                    ImageUrl = x.Category.ImageUrl
                })
                .ToList();
        }

        // Карточки макета с 2х2 товарами.
        private static List<HomeCategoryBlockDto> BuildCategoryBlocks(
            List<Category> categories,
            Dictionary<Guid, List<Category>> subcategoriesByParentId,
            Dictionary<Guid, List<Product>> productsByCategory,
            List<Guid> excludedCategoryIds
            )
        {
            // Берет любые категории кроме TopCategories
            // Если у категории есть дети -> берем товары по всей ветке
            var categoryCandidates = categories
                .Where(x => !excludedCategoryIds.Contains(x.Id))
                .Select(x => new
                {
                    Category = x,
                    Products = GetCategoryBlockProducts(x, subcategoriesByParentId, productsByCategory)
                })
                .Where(x => x.Products.Count > 0)
                .OrderByDescending(x => x.Products.Count)
                .ThenByDescending(x => x.Products.Sum(p => p.RatingCount))
                .ThenBy(x => x.Category.Name)
                .Take(CategoryBlocksCount)
                .Select(x => new HomeCategoryBlockDto
                {
                    CategoryId = x.Category.Id,
                    CategoryName = x.Category.Name,
                    Products = SortProductsByRating(x.Products)
                        .Take(CategoryBlockProductsCount)
                        .Select(p => p.ToCardDto())
                        .ToList()
                })
                .ToList();

            return categoryCandidates;
        }


        private static List<HomeCategoryCardDto> BuildPopularCategories(
            List<Category> categories,
            Dictionary<Guid, List<Category>> subcategoriesByParentId,
            Dictionary<Guid, List<Product>> productsByCategory,
            List<Guid> excludedCategoryIds
            )
        {
            return categories
                .Where(x => !excludedCategoryIds.Contains(x.Id))
                .Select(x => new
                {
                    Category = x,
                    Products = GetProductsInCategoryTree(x.Id, subcategoriesByParentId, productsByCategory)
                })
                .Where(x => x.Products.Count > 0)
                .OrderByDescending(x => x.Products.Count)
                .ThenByDescending(x => x.Products.Sum(p => p.RatingCount))
                .ThenBy(x => x.Category.Name)
                .Take(PopularCategoriesCount)
                .Select(x => new HomeCategoryCardDto
                {
                    Id = x.Category.Id,
                    Name = x.Category.Name,
                    ImageUrl = x.Category.ImageUrl
                })
                .ToList();
        }


        private static List<ProductCardDto> BuildProductCards(IEnumerable<Product> products, int take)
        {
            List<Product> selectedProducts = SortProductsByRating(products)
                .Take(take).ToList();

            return selectedProducts.Select(x => x.ToCardDto()).ToList();
        }


        private static IEnumerable<Product> SortProductsByRating(IEnumerable<Product> products)
        {
            return products
                .OrderByDescending(x => x.RatingCount)
                .ThenByDescending(GetAverageRating)
                .ThenByDescending(x => x.CreatedAt)
                .ThenBy(x => x.Title);
        }

        private static bool HasMinRating(Product product, decimal minRating)
        {
            return product.RatingCount > 0 && GetAverageRating(product) > minRating;
        }

        private static decimal GetAverageRating(Product product)
        {
            if (product.RatingCount <= 0)
                return 0m;

            return (decimal)product.RatingSum / product.RatingCount;
        }


        private static List<Product> GetCategoryBlockProducts(
            Category category,
            Dictionary<Guid, List<Category>> subcategoriesByParentId,
            Dictionary<Guid, List<Product>> productsByCategory
            )
        {
            if (subcategoriesByParentId.ContainsKey(category.Id))
                return GetProductsInCategoryTree(category.Id, subcategoriesByParentId, productsByCategory);

            if (!productsByCategory.ContainsKey(category.Id))
                return new List<Product>();

            return productsByCategory[category.Id];
        }


        private static List<Product> GetProductsInCategoryTree(
            Guid categoryId,
            Dictionary<Guid, List<Category>> subcategoriesByParentId,
            Dictionary<Guid, List<Product>> productsByCategory
            )
        {
            var result = new List<Product>();

            // товары самой категории
            if (productsByCategory.TryGetValue(categoryId, out var directProducts))
                result.AddRange(directProducts);

            // товары подкатегорий
            if (subcategoriesByParentId.TryGetValue(categoryId, out var children))
            {
                foreach (var child in children)
                {
                    result.AddRange(
                        GetProductsInCategoryTree(
                            child.Id,
                            subcategoriesByParentId,
                            productsByCategory));
                }
            }

            return result;
        }
    }


}