namespace AMZN.Data.Entities;

public class PaymentMethod
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CardNumber { get; set; } = null!;
    public DateOnly ExpirationDate { get; set; }
    //public string Cvv { get; set; } = null!;
    public bool IsDefault { get; set; }
    public User User { get; set; } = null!;
}