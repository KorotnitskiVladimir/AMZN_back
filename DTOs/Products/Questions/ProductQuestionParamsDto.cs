using System.ComponentModel.DataAnnotations;

namespace AMZN.DTOs.Products.Questions
{
    public class ProductQuestionParamsDto
    {
        private const int PageSizeMax = 50;

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, PageSizeMax)]
        public int PageSize { get; set; } = 10;
    }
}
