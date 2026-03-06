namespace AMZN.Data.Entities
{
    public class Brand
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public List<Product> Products { get; set; } = new();
    }
}
