using AMZN.Data.Entities;
using AMZN.DTOs.Common;
using AMZN.DTOs.Home;
using AMZN.DTOs.Products;

namespace AMZN.Shared.Mapping
{
    public static class ProductMapper
    {

        public static HomeProductDto ToHomeDto(this Product p)
        {
            return new HomeProductDto
            {
                Id = p.Id,
                Title = p.Title,
                Rating = CalcRatingHalfStep(p.RatingSum, p.RatingCount),
                Price = new PriceDto
                {
                    Current = p.CurrentPrice,
                    Original = CalcOriginalPrice(p.CurrentPrice, p.OriginalPrice),
                },

                Image = new ImageUrlDto { Url = p.PrimaryImageUrl }
            };
        }

        public static ProductDetailsDto ToDetailsDto(this Product p)
        {
            return new ProductDetailsDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Rating = CalcRatingHalfStep(p.RatingSum, p.RatingCount),
                RatingCount = p.RatingCount,
                Price = new PriceDto
                {
                    Current = p.CurrentPrice,
                    Original = CalcOriginalPrice(p.CurrentPrice, p.OriginalPrice)
                },

                PrimaryImage = new ImageUrlDto { Url = p.PrimaryImageUrl },

                Images = p.Images
                    .OrderBy(i => i.SortOrder)
                    .Select(x => new ImageDto { Url = x.Url, SortOrder = x.SortOrder })
                    .ToList(),

                Category = new CategoryDto
                {
                    Id = p.CategoryId,
                    Name = p.Category?.Name ?? "",
                }
            };

        }


        // округляет средний рейтинг до 0.5 для UI звезд
        private static decimal CalcRatingHalfStep(int sum, int count)
        {
            if (count <= 0) return 0m;

            decimal avg = (decimal)sum / count;
            return Math.Round(avg * 2m, MidpointRounding.AwayFromZero) / 2m;
        }

        // original = цена до скидки, если скидки нет -> равна Current
        private static decimal CalcOriginalPrice(decimal current, decimal? original)
        {
            if (original != null && original.Value > current)
                return original.Value;

            return current;
        }


    }
}
