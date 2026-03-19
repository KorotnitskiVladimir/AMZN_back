namespace AMZN.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public DateOnly? BirthDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        
        public ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();


    }

}
