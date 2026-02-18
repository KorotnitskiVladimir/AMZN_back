namespace AMZN.Data.Entities
{
    public class ProductImage
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }


        public string Url { get; set; } = null!;
        public int SortOrder { get; set; } = 0;


        //
        public Product Product { get; set; } = null!;
    }
}
