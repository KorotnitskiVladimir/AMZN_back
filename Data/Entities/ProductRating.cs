namespace AMZN.Data.Entities
{
    public class ProductRating
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }

        public byte Value { get; set; }         // от 1 до 5


        //
        public Product Product { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
