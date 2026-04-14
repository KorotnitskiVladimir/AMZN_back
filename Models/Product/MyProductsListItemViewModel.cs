namespace AMZN.Models.Product
{
    public class MyProductsListItemViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public decimal CurrentPrice { get; set; }
        public int StockQuantity { get; set; }
        public string PrimaryImageUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
