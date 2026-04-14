namespace AMZN.Models.Product
{
    public class ExistingProductImageViewModel
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = null!;
        public int SortOrder { get; set; }
    }
}
