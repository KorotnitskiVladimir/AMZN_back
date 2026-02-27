namespace AMZN.DTOs.Common
{
    public class ImageDto
    {
        public string Url { get; set; } = null!;
        public int? SortOrder { get; set; }     // для primary img не нужно
    }
}
