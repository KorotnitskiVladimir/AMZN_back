using AMZN.Data.Entities;
using AMZN.DTOs.Home;

namespace AMZN.Shared.Mapping
{
    public static class ProductMapper
    {
        public static HomeProductDto ToHomeDto(this Product p)
        {
            // средний рейтинг
            decimal avgRating = p.RatingCount > 0
                ? (decimal)p.RatingSum / p.RatingCount
                : 0m;

            // округление до 0.5 для пол звезды в UI
            decimal rating = Math.Round(avgRating * 2m, MidpointRounding.AwayFromZero) / 2m;

            // Если есть скидка - показываем старую цену, иначе текущую
            decimal originalPrice = p.CurrentPrice;

            if (p.OriginalPrice.HasValue && p.OriginalPrice.Value > p.CurrentPrice)
            {
                originalPrice = p.OriginalPrice.Value;
            }


            return new HomeProductDto
            {
                Id = p.Id,
                Name = p.Title,
                Rating = rating,
                Price = new PriceDto { Current = p.CurrentPrice, Original = originalPrice },
                Image = new ImageDto { Url = p.PrimaryImageUrl }
            };
        }


    }
}
